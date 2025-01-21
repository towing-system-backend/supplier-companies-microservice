using Application.Core;

namespace SupplierCompany.Domain
{
    public class PolicyRegisteredEvent(string publisherId, string type, PolicyRegistered context) : DomainEvent(publisherId, type, context) { }

    public class PolicyRegistered(
        string id,
        string title,
        int coverageAmount,
        int coverageDistance,
        decimal price,
        string type,
        DateOnly issuanceDate,
        DateOnly expirationDate
    )
    {
        public readonly string PolicyId = id;
        public readonly string Title = title;
        public readonly int CoverageAmount = coverageAmount;
        public readonly int CoverageDistance = coverageDistance;
        public readonly decimal Price = price;
        public readonly string Type = type;
        public readonly DateOnly IssuanceDate = issuanceDate;
        public readonly DateOnly ExpirationDate = expirationDate;
        public static PolicyRegisteredEvent CreateEvent(
            SupplierCompanyId publisherId,
            PolicyId policyId,
            PolicyTitle title,
            PolicyCoverageAmount coverageAmount,
            PolicyCoverageDistance coverageDistance,
            PolicyPrice price,
            PolicyType type,
            PolicyIssuanceDate issuanceDate,
            PolicyExpirationDate expirationDate)
        {
            return new PolicyRegisteredEvent(
            publisherId.GetValue(),
            typeof(PolicyRegistered).Name,
            new PolicyRegistered(
                    policyId.GetValue(),
                    title.GetValue(),
                    coverageAmount.GetValue(),
                    coverageDistance.GetValue(),
                    price.GetValue(),
                    type.GetValue(),
                    issuanceDate.GetValue(),
                    expirationDate.GetValue()
                )
            );
        }
    }
}
