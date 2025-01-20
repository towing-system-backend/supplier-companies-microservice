using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidPolicyPriceException : DomainException
    {
        public InvalidPolicyPriceException() : base("Invalid policy price.") { }
    }
}
