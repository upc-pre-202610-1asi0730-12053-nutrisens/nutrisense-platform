using Cortex.Mediator;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Errors;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.BodyHealthMetrics.Application.Internal.CommandServices;

/// <summary>Orchestrates all body-metrics write use cases and the post-registration calculation saga.</summary>
public class BodyMetricsCommandService(
    IBodyMetricsRepository bodyMetricsRepository,
    IUnitOfWork unitOfWork,
    IBodyMetricsCalculator calculator,
    ILogger<BodyMetricsCommandService> logger,
    IMediator mediator) : IBodyMetricsCommandService
{
    public async Task<Result<BodyMetrics, RegisterBodyMetricsError>> Handle(RegisterBodyMetricsCommand command)
    {
        try
        {
            if (await bodyMetricsRepository.FindByUserIdAsync(command.UserId) is not null)
                return new Result<BodyMetrics, RegisterBodyMetricsError>.Failure(RegisterBodyMetricsError.AlreadyExists);

            BodyMetrics bodyMetrics;
            try
            {
                bodyMetrics = new BodyMetrics(command);
            }
            catch (ArgumentException)
            {
                return new Result<BodyMetrics, RegisterBodyMetricsError>.Failure(RegisterBodyMetricsError.InvalidData);
            }

            await bodyMetricsRepository.AddAsync(bodyMetrics);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new BodyMetricsRegistered(command.UserId));

            // Saga: BodyMetricsRegistered → BMI → BMR → TDEE → DailyCaloricGoal
            await RunCalculationChain(bodyMetrics, command);

            return new Result<BodyMetrics, RegisterBodyMetricsError>.Success(bodyMetrics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering body metrics for user {UserId}", command.UserId);
            return new Result<BodyMetrics, RegisterBodyMetricsError>.Failure(RegisterBodyMetricsError.UnexpectedError);
        }
    }

    public async Task<Result<BodyMetrics, UpdateWeightError>> Handle(UpdateWeightCommand command)
    {
        try
        {
            var bodyMetrics = await bodyMetricsRepository.FindByUserIdAsync(command.UserId);
            if (bodyMetrics is null)
                return new Result<BodyMetrics, UpdateWeightError>.Failure(UpdateWeightError.BodyMetricsNotFound);

            try
            {
                bodyMetrics.LogWeight(command.WeightKg, command.Note);
            }
            catch (ArgumentException)
            {
                return new Result<BodyMetrics, UpdateWeightError>.Failure(UpdateWeightError.InvalidData);
            }

            bodyMetricsRepository.Update(bodyMetrics);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new WeightUpdated(command.UserId, command.WeightKg));

            // Saga: WeightUpdated → CalculateBmi
            var weight = bodyMetrics.GetCurrentWeightKg();
            if (weight.HasValue)
            {
                var bmi = calculator.CalculateBmi(weight.Value, bodyMetrics.HeightCm);
                bodyMetrics.SetBmi(bmi);
                bodyMetricsRepository.Update(bodyMetrics);
                await unitOfWork.CompleteAsync();
                await mediator.PublishAsync(new BmiCalculated(command.UserId, bmi.Value));
            }

            return new Result<BodyMetrics, UpdateWeightError>.Success(bodyMetrics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating weight for user {UserId}", command.UserId);
            return new Result<BodyMetrics, UpdateWeightError>.Failure(UpdateWeightError.UnexpectedError);
        }
    }

    public async Task<Result<BodyMetrics, RegisterBodyMeasurementError>> Handle(RegisterBodyMeasurementCommand command)
    {
        try
        {
            var bodyMetrics = await bodyMetricsRepository.FindByUserIdAsync(command.UserId);
            if (bodyMetrics is null)
                return new Result<BodyMetrics, RegisterBodyMeasurementError>.Failure(RegisterBodyMeasurementError.BodyMetricsNotFound);

            try
            {
                bodyMetrics.LogMeasurement(command.WaistCm, command.NeckCm);
            }
            catch (ArgumentException)
            {
                return new Result<BodyMetrics, RegisterBodyMeasurementError>.Failure(RegisterBodyMeasurementError.InvalidData);
            }

            bodyMetricsRepository.Update(bodyMetrics);
            await unitOfWork.CompleteAsync();

            return new Result<BodyMetrics, RegisterBodyMeasurementError>.Success(bodyMetrics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering body measurement for user {UserId}", command.UserId);
            return new Result<BodyMetrics, RegisterBodyMeasurementError>.Failure(RegisterBodyMeasurementError.UnexpectedError);
        }
    }

    public async Task<Result<BodyMetrics, SetHealthGoalError>> Handle(SetHealthGoalCommand command)
    {
        try
        {
            var bodyMetrics = await bodyMetricsRepository.FindByUserIdAsync(command.UserId);
            if (bodyMetrics is null)
                return new Result<BodyMetrics, SetHealthGoalError>.Failure(SetHealthGoalError.BodyMetricsNotFound);

            try
            {
                bodyMetrics.SetHealthGoal(command);
            }
            catch (ArgumentException)
            {
                return new Result<BodyMetrics, SetHealthGoalError>.Failure(SetHealthGoalError.InvalidData);
            }

            bodyMetricsRepository.Update(bodyMetrics);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new GoalDefinedInMetrics(command.UserId, command.Goal));

            // Eagerly calculate daily caloric goal if TDEE is already available
            if (bodyMetrics.Tdee.HasValue)
            {
                var activeGoal = bodyMetrics.GetActiveGoal();
                if (activeGoal is not null)
                {
                    var adjustment = calculator.CalculateCaloricAdjustment(activeGoal.Goal.Value, activeGoal.WeeklyRateKg.Value);
                    var macros = calculator.CalculateDailyCaloricGoal(bodyMetrics.Tdee.Value, activeGoal.Goal.Value, activeGoal.WeeklyRateKg.Value);
                    bodyMetrics.SetDailyCaloricGoal(macros, adjustment);
                    bodyMetricsRepository.Update(bodyMetrics);
                    await unitOfWork.CompleteAsync();
                    await mediator.PublishAsync(new DailyCaloricGoalSet(command.UserId, macros.Calories, macros.ProteinG, macros.CarbsG, macros.FatG, macros.FiberG));
                }
            }

            return new Result<BodyMetrics, SetHealthGoalError>.Success(bodyMetrics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting health goal for user {UserId}", command.UserId);
            return new Result<BodyMetrics, SetHealthGoalError>.Failure(SetHealthGoalError.UnexpectedError);
        }
    }

    public async Task<Result<BodyMetrics, CalculateDailyCaloricGoalError>> Handle(CalculateDailyCaloricGoalCommand command)
    {
        try
        {
            var bodyMetrics = await bodyMetricsRepository.FindByUserIdAsync(command.UserId);
            if (bodyMetrics is null)
                return new Result<BodyMetrics, CalculateDailyCaloricGoalError>.Failure(CalculateDailyCaloricGoalError.BodyMetricsNotFound);

            var activeGoal = bodyMetrics.GetActiveGoal();
            if (activeGoal is null)
                return new Result<BodyMetrics, CalculateDailyCaloricGoalError>.Failure(CalculateDailyCaloricGoalError.NoActiveGoal);

            if (!bodyMetrics.Tdee.HasValue)
                return new Result<BodyMetrics, CalculateDailyCaloricGoalError>.Failure(CalculateDailyCaloricGoalError.TdeeNotCalculated);

            var adjustment = calculator.CalculateCaloricAdjustment(activeGoal.Goal.Value, activeGoal.WeeklyRateKg.Value);
            var macros = calculator.CalculateDailyCaloricGoal(bodyMetrics.Tdee.Value, activeGoal.Goal.Value, activeGoal.WeeklyRateKg.Value);
            bodyMetrics.SetDailyCaloricGoal(macros, adjustment);

            bodyMetricsRepository.Update(bodyMetrics);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new DailyCaloricGoalSet(command.UserId, macros.Calories, macros.ProteinG, macros.CarbsG, macros.FatG, macros.FiberG));

            return new Result<BodyMetrics, CalculateDailyCaloricGoalError>.Success(bodyMetrics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating daily caloric goal for user {UserId}", command.UserId);
            return new Result<BodyMetrics, CalculateDailyCaloricGoalError>.Failure(CalculateDailyCaloricGoalError.UnexpectedError);
        }
    }

    // Subflow 2.1/2.2: BMI → BMR → TDEE → DailyCaloricGoal. Events are collected while the
    // aggregate is mutated, persisted once, and published only after CompleteAsync() so no
    // event escapes before its state change is committed.
    private async Task RunCalculationChain(BodyMetrics bodyMetrics, RegisterBodyMetricsCommand command)
    {
        var weight = bodyMetrics.GetCurrentWeightKg();
        if (weight is null) return;

        var events = new List<IEvent>();

        var bmi = calculator.CalculateBmi(weight.Value, bodyMetrics.HeightCm);
        bodyMetrics.SetBmi(bmi);
        events.Add(new BmiCalculated(bodyMetrics.UserId, bmi.Value));

        if (bodyMetrics.DateOfBirth.HasValue && bodyMetrics.BiologicalSex is not null)
        {
            var bmr = calculator.CalculateBmr(weight.Value, bodyMetrics.HeightCm, bodyMetrics.DateOfBirth.Value, bodyMetrics.BiologicalSex);
            bodyMetrics.SetBmr(bmr);
            events.Add(new BmrCalculated(bodyMetrics.UserId, bmr));

            if (bodyMetrics.ActivityLevel is not null)
            {
                var tdee = calculator.CalculateTdee(bmr, bodyMetrics.ActivityLevel);
                bodyMetrics.SetTdee(tdee);
                events.Add(new TdeeCalculated(bodyMetrics.UserId, tdee));

                // Onboarding carries the goal intent: seed a default active goal so the daily
                // caloric/macro targets are computed here, without a separate goal-setting step.
                if (bodyMetrics.GetActiveGoal() is null && !string.IsNullOrWhiteSpace(command.Goal))
                    TrySeedDefaultGoal(bodyMetrics, command, weight.Value);

                var activeGoal = bodyMetrics.GetActiveGoal();
                if (activeGoal is not null)
                {
                    var adjustment = calculator.CalculateCaloricAdjustment(activeGoal.Goal.Value, activeGoal.WeeklyRateKg.Value);
                    var macros = calculator.CalculateDailyCaloricGoal(tdee, activeGoal.Goal.Value, activeGoal.WeeklyRateKg.Value);
                    bodyMetrics.SetDailyCaloricGoal(macros, adjustment);
                    events.Add(new DailyCaloricGoalSet(bodyMetrics.UserId, macros.Calories, macros.ProteinG, macros.CarbsG, macros.FatG, macros.FiberG));
                }
            }
        }

        bodyMetricsRepository.Update(bodyMetrics);
        await unitOfWork.CompleteAsync();

        foreach (var @event in events)
            await mediator.PublishAsync(@event);
    }

    // Establishes a default active goal from the onboarding intent. The macro targets
    // depend only on TDEE + goal + weekly rate, so the target weight is a sensible
    // placeholder. Invalid goal values are logged and skipped (metrics still register).
    private void TrySeedDefaultGoal(BodyMetrics bodyMetrics, RegisterBodyMetricsCommand command, decimal currentWeightKg)
    {
        try
        {
            var weeklyRate = command.WeeklyRateKg ?? DefaultWeeklyRate(command.Goal!);
            var targetWeight = DefaultTargetWeight(command.Goal!, currentWeightKg);
            bodyMetrics.SetHealthGoal(new SetHealthGoalCommand(bodyMetrics.UserId, command.Goal!, targetWeight, weeklyRate));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex,
                "Could not seed default goal '{Goal}' for user {UserId}; registering metrics without a goal.",
                command.Goal, command.UserId);
        }
    }

    private static decimal DefaultWeeklyRate(string goal) =>
        goal == "muscle-gain" ? 0.25m : 0.5m;

    private static decimal DefaultTargetWeight(string goal, decimal currentWeightKg) =>
        Math.Round(goal == "muscle-gain" ? currentWeightKg * 1.1m : currentWeightKg * 0.9m, 2);
}
