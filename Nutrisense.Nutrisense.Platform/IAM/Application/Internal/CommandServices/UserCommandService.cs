using Cortex.Mediator;
using Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.IAM.Application.Errors;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IHashingService hashingService,
    ITokenService tokenService,
    ILogger<UserCommandService> logger,
    IMediator mediator) : IUserCommandService
{
    public async Task<Result<User, RegisterUserError>> Handle(RegisterUserCommand command)
    {
        Email email;
        try
        {
            email = new Email(command.Email);
        }
        catch (ArgumentException)
        {
            return new Result<User, RegisterUserError>.Failure(RegisterUserError.InvalidEmail);
        }

        try
        {
            _ = new Password(command.Password);
        }
        catch (ArgumentException)
        {
            return new Result<User, RegisterUserError>.Failure(RegisterUserError.WeakPassword);
        }

        try
        {
            if (await userRepository.ExistsByEmailAsync(email))
                return new Result<User, RegisterUserError>.Failure(RegisterUserError.EmailAlreadyTaken);

            var passwordHash = hashingService.Hash(command.Password);
            var user = new User(command, passwordHash);

            await userRepository.AddAsync(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new UserRegistered(user.Id, user.Email.Value, user.PreferredLanguage.Value));

            return new Result<User, RegisterUserError>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering user with email {Email}", command.Email);
            return new Result<User, RegisterUserError>.Failure(RegisterUserError.UnexpectedError);
        }
    }

    public async Task<Result<LoginResult, LoginUserError>> Handle(LoginUserCommand command)
    {
        try
        {
            Email email;
            try
            {
                email = new Email(command.Email);
            }
            catch (ArgumentException)
            {
                return new Result<LoginResult, LoginUserError>.Failure(LoginUserError.InvalidCredentials);
            }

            var user = await userRepository.FindByEmailAsync(email);
            if (user is null)
                return new Result<LoginResult, LoginUserError>.Failure(LoginUserError.UserNotFound);

            if (!hashingService.Verify(command.Password, user.PasswordHash))
                return new Result<LoginResult, LoginUserError>.Failure(LoginUserError.InvalidCredentials);

            var session = user.AddSession(command.DeviceLabel);
            userRepository.Update(user);
            await unitOfWork.CompleteAsync();

            var token = tokenService.Generate(user, session.Id);
            await mediator.PublishAsync(new UserLoggedIn(user.Id, session.Id));

            return new Result<LoginResult, LoginUserError>.Success(new LoginResult(user.Id, token, session.Id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for email {Email}", command.Email);
            return new Result<LoginResult, LoginUserError>.Failure(LoginUserError.UnexpectedError);
        }
    }

    public async Task<Result<bool, LogoutUserError>> Handle(LogoutUserCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<bool, LogoutUserError>.Failure(LogoutUserError.UnexpectedError);

            try
            {
                user.EndSession(command.SessionId);
            }
            catch (InvalidOperationException)
            {
                return new Result<bool, LogoutUserError>.Failure(LogoutUserError.SessionNotFound);
            }

            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new UserLoggedOut(user.Id, command.SessionId));

            return new Result<bool, LogoutUserError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging out session {SessionId} for user {UserId}", command.SessionId, command.UserId);
            return new Result<bool, LogoutUserError>.Failure(LogoutUserError.UnexpectedError);
        }
    }

    public async Task<Result<User, UpdateProfileError>> Handle(UpdateProfileCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<User, UpdateProfileError>.Failure(UpdateProfileError.UserNotFound);

            try
            {
                user.Apply(command);
            }
            catch (ArgumentException)
            {
                return new Result<User, UpdateProfileError>.Failure(UpdateProfileError.InvalidData);
            }

            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new ProfileUpdated(user.Id));

            return new Result<User, UpdateProfileError>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating profile for user {UserId}", command.UserId);
            return new Result<User, UpdateProfileError>.Failure(UpdateProfileError.UnexpectedError);
        }
    }

    public async Task<Result<User, SetHealthGoalError>> Handle(SetHealthGoalCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<User, SetHealthGoalError>.Failure(SetHealthGoalError.UserNotFound);

            try
            {
                user.Apply(command);
            }
            catch (ArgumentException)
            {
                return new Result<User, SetHealthGoalError>.Failure(SetHealthGoalError.InvalidGoal);
            }

            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new GoalDefined(user.Id, command.Goal));

            return new Result<User, SetHealthGoalError>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting health goal for user {UserId}", command.UserId);
            return new Result<User, SetHealthGoalError>.Failure(SetHealthGoalError.UnexpectedError);
        }
    }

    public async Task<Result<User, SetDietaryRestrictionsError>> Handle(SetDietaryRestrictionsCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<User, SetDietaryRestrictionsError>.Failure(SetDietaryRestrictionsError.UserNotFound);

            user.Apply(command);
            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new RestrictionsConfigured(user.Id, command.Restrictions));

            return new Result<User, SetDietaryRestrictionsError>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting dietary restrictions for user {UserId}", command.UserId);
            return new Result<User, SetDietaryRestrictionsError>.Failure(SetDietaryRestrictionsError.UnexpectedError);
        }
    }

    /// <summary>
    /// Permanently deletes a user account.
    /// EF cascade (configured in UserEntityTypeConfiguration) removes Sessions and
    /// DietaryRestrictions automatically. A UserDeleted event is published so that
    /// cross-BC subscribers (BodyMetrics, Subscriptions, SmartRecommendations, etc.)
    /// can purge their own data. No migration is required.
    /// </summary>
    public async Task<Result<bool, DeleteUserError>> Handle(DeleteUserCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<bool, DeleteUserError>.Failure(DeleteUserError.UserNotFound);

            userRepository.Remove(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new UserDeleted(command.UserId));

            return new Result<bool, DeleteUserError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user {UserId}", command.UserId);
            return new Result<bool, DeleteUserError>.Failure(DeleteUserError.UnexpectedError);
        }
    }
}
