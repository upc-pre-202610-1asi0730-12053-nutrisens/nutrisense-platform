using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Commands;
using Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Entities;

namespace Nutrisense.Nutrisense.Platform.Subscriptions.Domain.Model.Aggregates;

public partial class UserSubscription
{
    public int Id { get; private set; }

    /// <summary>Reference to IAM.User — no FK, cross-BC boundary.</summary>
    public int UserId { get; private set; }

    public int PlanId { get; private set; }
    public string PlanKey { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string BillingPeriod { get; private set; } = null!;
    public DateTimeOffset PeriodStart { get; private set; }
    public DateTimeOffset PeriodEnd { get; private set; }
    public bool CancelAtPeriodEnd { get; private set; }
    public string? StripeSubscriptionId { get; private set; }
    public DateTimeOffset? LastPlanChangeAt { get; private set; }
    public int? PaymentMethodId { get; private set; }

    private List<PaymentRecord> _paymentRecords = new();
    public IReadOnlyCollection<PaymentRecord> PaymentRecords => _paymentRecords.AsReadOnly();

    protected UserSubscription() { }

    public UserSubscription(SelectSubscriptionPlanCommand command, int planId)
    {
        UserId = command.UserId;
        PlanId = planId;
        PlanKey = command.PlanKey;
        PaymentMethodId = command.PaymentMethodId;
        Status = "pending-payment";
        BillingPeriod = command.BillingPeriod;
        PeriodStart = DateTimeOffset.UtcNow;
        PeriodEnd = ComputePeriodEnd(command.BillingPeriod);
        LastPlanChangeAt = DateTimeOffset.UtcNow;
        _paymentRecords = new List<PaymentRecord>();
    }

    public void ApplyPayment(PaymentRecord record)
    {
        _paymentRecords.Add(record);
        Status = "active";
    }

    public void ApplyActivation()
    {
        Status = "active";
    }

    public void ApplyCancel(bool cancelAtPeriodEnd)
    {
        Status = "cancelled";
        CancelAtPeriodEnd = cancelAtPeriodEnd;
    }

    public void ApplyRenewal(PaymentRecord record, DateTimeOffset newPeriodEnd)
    {
        _paymentRecords.Add(record);
        PeriodStart = DateTimeOffset.UtcNow;
        PeriodEnd = newPeriodEnd;
    }

    /// <summary>
    /// Upgrade path: charges have already been collected. Resets the billing period from now.
    /// </summary>
    public void ApplyPlanChangeWithPayment(string newPlanKey, int newPlanId, PaymentRecord record, string billingPeriod)
    {
        _paymentRecords.Add(record);
        PlanKey = newPlanKey;
        PlanId = newPlanId;
        BillingPeriod = billingPeriod;
        PeriodStart = DateTimeOffset.UtcNow;
        PeriodEnd = ComputePeriodEnd(billingPeriod);
        LastPlanChangeAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Downgrade path: no charge. PeriodEnd is unchanged; billing period may change on next renewal.
    /// </summary>
    public void ApplyPlanChangeWithoutPayment(string newPlanKey, int newPlanId, string billingPeriod)
    {
        PlanKey = newPlanKey;
        PlanId = newPlanId;
        BillingPeriod = billingPeriod;
        LastPlanChangeAt = DateTimeOffset.UtcNow;
    }

    private static DateTimeOffset ComputePeriodEnd(string billingPeriod) =>
        billingPeriod == ValueObjects.BillingPeriod.Annual
            ? DateTimeOffset.UtcNow.AddYears(1)
            : DateTimeOffset.UtcNow.AddMonths(1);
}
