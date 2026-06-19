using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.ValueObjects;

namespace Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Aggregates;

public partial class User
{
    public UserId Id { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public PersonName Name { get; private set; } = null!;
    public DateOfBirth? DateOfBirth { get; private set; }
    public BiologicalSex? BiologicalSex { get; private set; }
    public Height? Height { get; private set; }
    public ActivityLevel? ActivityLevel { get; private set; }
    public PreferredUnits PreferredUnits { get; private set; } = null!;
    public PreferredLanguage PreferredLanguage { get; private set; } = null!;
    public GoalIntent? GoalIntent { get; private set; }
    public List<string> MedicalConditions { get; private set; } = new();
    public List<UserSession> Sessions { get; private set; } = new();
    public List<DietaryRestriction> DietaryRestrictions { get; private set; } = new();

    protected User() { }

    public User(RegisterUserCommand command, string passwordHash)
    {
        Email = new Email(command.Email);
        PasswordHash = passwordHash;
        Name = new PersonName(command.FirstName, command.LastName);
        PreferredUnits = new PreferredUnits("metric");
        PreferredLanguage = new PreferredLanguage(command.PreferredLanguage);
    }

    public void Apply(UpdateProfileCommand command)
    {
        if (command.FirstName is not null || command.LastName is not null)
        {
            var first = command.FirstName ?? Name.FirstName;
            var last = command.LastName ?? Name.LastName;
            Name = new PersonName(first, last);
        }

        if (command.DateOfBirth.HasValue)
            DateOfBirth = new DateOfBirth(command.DateOfBirth.Value);

        if (command.BiologicalSex is not null)
            BiologicalSex = new BiologicalSex(command.BiologicalSex);

        if (command.HeightCm.HasValue)
            Height = new Height(command.HeightCm.Value);

        if (command.ActivityLevel is not null)
            ActivityLevel = new ActivityLevel(command.ActivityLevel);

        if (command.PreferredUnits is not null)
            PreferredUnits = new PreferredUnits(command.PreferredUnits);

        if (command.PreferredLanguage is not null)
            PreferredLanguage = new PreferredLanguage(command.PreferredLanguage);

        if (command.MedicalConditions is not null)
            MedicalConditions = command.MedicalConditions.ToList();
    }

    public void Apply(SetHealthGoalCommand command)
    {
        GoalIntent = new GoalIntent(command.Goal);
    }

    public void Apply(SetDietaryRestrictionsCommand command)
    {
        DietaryRestrictions.Clear();
        foreach (var restriction in command.Restrictions)
            DietaryRestrictions.Add(new DietaryRestriction(Id, restriction));
    }

    public UserSession AddSession(string? deviceLabel)
    {
        var session = new UserSession(Id, deviceLabel ?? "Unknown Device");
        Sessions.Add(session);
        return session;
    }

    public void EndSession(int sessionId)
    {
        var session = Sessions.FirstOrDefault(s => s.Id == sessionId)
            ?? throw new InvalidOperationException($"Session {sessionId} not found for user {Id}.");
        session.End();
    }
}
