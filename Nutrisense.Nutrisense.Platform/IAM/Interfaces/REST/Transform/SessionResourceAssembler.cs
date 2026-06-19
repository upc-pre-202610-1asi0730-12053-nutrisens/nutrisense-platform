using Nutrisense.Nutrisense.Platform.IAM.Domain.Model.Entities;
using Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Resources;

namespace Nutrisense.Nutrisense.Platform.IAM.Interfaces.REST.Transform;

public static class SessionResourceAssembler
{
    public static SessionResource ToResource(UserSession session) =>
        new(session.Id, session.DeviceLabel, session.IsCurrent, session.CreatedAt, session.LastActiveAt);
}
