using Cortex.Mediator;
using Nutrisense.Nutrisense.Platform.IAM.Application.CommandServices;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Errors;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Events;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Repositories;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Services;
using Nutrisense.Nutrisense.Platform.Shared.Application.Patterns;
using Nutrisense.Nutrisense.Platform.Shared.Domain.Repositories;

namespace Nutrisense.Nutrisense.Platform.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    IHashingService hashingService,
    ITokenService tokenService,
    ILogger<UserCommandService> logger,
    IMediator mediator) : IUserCommandService
{
    public async Task<Result<User, IamError>> Handle(RegisterUserCommand command)
    {
        Email email;
        try
        {
            email = new Email(command.Email);
        }
        catch (ArgumentException)
        {
            return new Result<User, IamError>.Failure(IamError.InvalidEmail);
        }

        try
        {
            _ = new Password(command.Password);
        }
        catch (ArgumentException)
        {
            return new Result<User, IamError>.Failure(IamError.WeakPassword);
        }

        try
        {
            if (await userRepository.ExistsByEmailAsync(email))
                return new Result<User, IamError>.Failure(IamError.EmailAlreadyTaken);

            var passwordHash = hashingService.Hash(command.Password);
            var user = new User(command, passwordHash);

            await userRepository.AddAsync(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new UserRegistered(user.Id, user.Email.Value, user.PreferredLanguage.Value));

            return new Result<User, IamError>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering user with email {Email}", command.Email);
            return new Result<User, IamError>.Failure(IamError.UnexpectedError);
        }
    }

    public async Task<Result<LoginResult, IamError>> Handle(LoginUserCommand command)
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
                return new Result<LoginResult, IamError>.Failure(IamError.InvalidCredentials);
            }

            var user = await userRepository.FindByEmailAsync(email);
            if (user is null)
                return new Result<LoginResult, IamError>.Failure(IamError.UserNotFound);

            if (!hashingService.Verify(command.Password, user.PasswordHash))
                return new Result<LoginResult, IamError>.Failure(IamError.InvalidCredentials);

            var session = user.AddSession(command.DeviceLabel);
            userRepository.Update(user);
            await unitOfWork.CompleteAsync();

            var token = tokenService.Generate(user, session.Id);
            await mediator.PublishAsync(new UserLoggedIn(user.Id, session.Id));

            return new Result<LoginResult, IamError>.Success(new LoginResult(user.Id, token, session.Id));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for email {Email}", command.Email);
            return new Result<LoginResult, IamError>.Failure(IamError.UnexpectedError);
        }
    }

    public async Task<Result<bool, IamError>> Handle(LogoutUserCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<bool, IamError>.Failure(IamError.UnexpectedError);

            try
            {
                user.EndSession(command.SessionId);
            }
            catch (InvalidOperationException)
            {
                return new Result<bool, IamError>.Failure(IamError.SessionNotFound);
            }

            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new UserLoggedOut(user.Id, command.SessionId));

            return new Result<bool, IamError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error logging out session {SessionId} for user {UserId}", command.SessionId, command.UserId);
            return new Result<bool, IamError>.Failure(IamError.UnexpectedError);
        }
    }

    public async Task<Result<User, IamError>> Handle(UpdateProfileCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<User, IamError>.Failure(IamError.UserNotFound);

            try
            {
                user.Apply(command);
            }
            catch (ArgumentException)
            {
                return new Result<User, IamError>.Failure(IamError.InvalidProfileData);
            }

            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new ProfileUpdated(user.Id));

            return new Result<User, IamError>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating profile for user {UserId}", command.UserId);
            return new Result<User, IamError>.Failure(IamError.UnexpectedError);
        }
    }

    public async Task<Result<User, IamError>> Handle(SetHealthGoalCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<User, IamError>.Failure(IamError.UserNotFound);

            try
            {
                user.Apply(command);
            }
            catch (ArgumentException)
            {
                return new Result<User, IamError>.Failure(IamError.InvalidGoal);
            }

            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new GoalDefined(user.Id, command.Goal));

            return new Result<User, IamError>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting health goal for user {UserId}", command.UserId);
            return new Result<User, IamError>.Failure(IamError.UnexpectedError);
        }
    }

    public async Task<Result<User, IamError>> Handle(SetDietaryRestrictionsCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<User, IamError>.Failure(IamError.UserNotFound);

            user.Apply(command);
            userRepository.Update(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new RestrictionsConfigured(user.Id, command.Restrictions));

            return new Result<User, IamError>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting dietary restrictions for user {UserId}", command.UserId);
            return new Result<User, IamError>.Failure(IamError.UnexpectedError);
        }
    }

    /// <summary>
    /// Permanently deletes a user account.
    /// EF cascade (configured in UserEntityTypeConfiguration) removes Sessions and
    /// DietaryRestrictions automatically. A UserDeleted event is published so that
    /// cross-BC subscribers (BodyMetrics, Subscriptions, SmartRecommendations, etc.)
    /// can purge their own data. No migration is required.
    /// </summary>
    public async Task<Result<bool, IamError>> Handle(DeleteUserCommand command)
    {
        try
        {
            var user = await userRepository.FindByIdAsync(command.UserId);
            if (user is null)
                return new Result<bool, IamError>.Failure(IamError.UserNotFound);

            userRepository.Remove(user);
            await unitOfWork.CompleteAsync();
            await mediator.PublishAsync(new UserDeleted(command.UserId));

            return new Result<bool, IamError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting user {UserId}", command.UserId);
            return new Result<bool, IamError>.Failure(IamError.UnexpectedError);
        }
    }

    /// <summary>
    /// Issues a password reset token and emails the recovery link.
    /// Always reports success regardless of whether the email is registered, to
    /// avoid leaking account existence (email enumeration). Any active token for
    /// the user is replaced so only the latest link works.
    /// </summary>
    public async Task<Result<bool, IamError>> Handle(RequestPasswordResetCommand command)
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
                // Don't reveal that the email is malformed/unknown.
                return new Result<bool, IamError>.Success(true);
            }

            var user = await userRepository.FindByEmailAsync(email);
            if (user is null)
                return new Result<bool, IamError>.Success(true);

            await passwordResetTokenRepository.DeleteByUserIdAsync(user.Id.Value);
            var token = PasswordResetToken.Create(user.Id.Value);
            await passwordResetTokenRepository.AddAsync(token);
            await unitOfWork.CompleteAsync();

            await emailService.SendPasswordResetEmailAsync(user.Email.Value, token.Token);

            return new Result<bool, IamError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error requesting password reset for email {Email}", command.Email);
            // Still report success so the response is indistinguishable from the happy path.
            return new Result<bool, IamError>.Success(true);
        }
    }

    /// <summary>
    /// Validates a reset token and sets the user's new password.
    /// The token must exist, be unused and unexpired; it is consumed on success.
    /// </summary>
    public async Task<Result<bool, IamError>> Handle(ResetPasswordCommand command)
    {
        try
        {
            var token = await passwordResetTokenRepository.FindByTokenAsync(command.Token);
            if (token is null)
                return new Result<bool, IamError>.Failure(IamError.InvalidResetToken);

            if (token.IsExpired())
                return new Result<bool, IamError>.Failure(IamError.ResetTokenExpired);

            if (!token.IsValid())
                return new Result<bool, IamError>.Failure(IamError.InvalidResetToken);

            try
            {
                _ = new Password(command.NewPassword);
            }
            catch (ArgumentException)
            {
                return new Result<bool, IamError>.Failure(IamError.WeakPassword);
            }

            var user = await userRepository.FindByIdAsync(token.UserId);
            if (user is null)
                return new Result<bool, IamError>.Failure(IamError.InvalidResetToken);

            user.ChangePassword(hashingService.Hash(command.NewPassword));
            userRepository.Update(user);

            token.MarkAsUsed();
            passwordResetTokenRepository.Update(token);

            await unitOfWork.CompleteAsync();

            return new Result<bool, IamError>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error resetting password");
            return new Result<bool, IamError>.Failure(IamError.UnexpectedError);
        }
    }
}
