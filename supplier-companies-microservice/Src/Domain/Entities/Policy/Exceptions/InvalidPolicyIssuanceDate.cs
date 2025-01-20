using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidPolicyIssuanceDateException : DomainException
    {
        public InvalidPolicyIssuanceDateException() : base("Invalid policy issuance date.") { }
    }
}
