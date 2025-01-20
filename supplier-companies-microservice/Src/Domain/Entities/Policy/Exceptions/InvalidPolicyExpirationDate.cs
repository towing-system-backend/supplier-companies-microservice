using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidPolicyExpirationDateException : DomainException
    {
        public InvalidPolicyExpirationDateException() : base("Invalid policy expiration date.") { }
    }
}
