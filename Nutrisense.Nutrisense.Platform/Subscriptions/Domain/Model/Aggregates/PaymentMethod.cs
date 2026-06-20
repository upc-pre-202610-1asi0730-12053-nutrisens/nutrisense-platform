using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

public partial class PaymentMethod
{
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    public string LastFour { get; private set; } = null!;
    public string Brand { get; private set; } = null!;
    public int ExpiryMonth { get; private set; }
    public int ExpiryYear { get; private set; }
    public string CardholderName { get; private set; } = null!;
    public string StripePaymentMethodId { get; private set; } = null!;

    protected PaymentMethod() { }

    public PaymentMethod(RegisterPaymentMethodCommand command)
    {
        UserId = command.UserId;
        LastFour = command.LastFour;
        Brand = command.Brand;
        ExpiryMonth = command.ExpiryMonth;
        ExpiryYear = command.ExpiryYear;
        CardholderName = command.CardholderName;
        StripePaymentMethodId = command.StripePaymentMethodId;
    }
}
