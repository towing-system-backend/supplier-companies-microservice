using Application.Core;

namespace SupplierCompany.Domain
{
    public record PolicyListType(string Id, string Title, int CoverageAmount, decimal Price, string Type, DateTime IssuanceDate, DateTime ExpirationDate);
    public class SupplierCompanyPoliciesUpdatedEvent(string publisherId, string type, SupplierCompanyPoliciesUpdated context) : DomainEvent(publisherId, type, context) { }

    public class SupplierCompanyPoliciesUpdated(List<PolicyListType> policies)
    {
        public readonly List<PolicyListType> Policies = policies;

        public static SupplierCompanyPoliciesUpdatedEvent CreateEvent(SupplierCompanyId publisherId, List<Policy> policies)
        {
            return new SupplierCompanyPoliciesUpdatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyPoliciesUpdated).Name,
                new SupplierCompanyPoliciesUpdated(
                    policies.Select(p => new PolicyListType(
                            p.GetId().GetValue(),
                            p.GetTitle().GetValue(),
                            p.GetCoverageAmount().GetValue(),
                            p.GetPrice().GetValue(),
                            p.GetType().GetValue(),
                            p.GetIssuanceDate().GetValue(),
                            p.GetExpirationDate().GetValue()
                        )
                    ).ToList()
                )
            );
        }
    }
}