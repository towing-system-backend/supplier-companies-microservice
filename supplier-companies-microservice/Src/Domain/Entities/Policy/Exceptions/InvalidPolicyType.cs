using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidPolicyTypeException : DomainException
    {
        public InvalidPolicyTypeException() : base("Invalid policy type.") { }
    }
}
