using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidPolicyIdException : DomainException
    {
        public InvalidPolicyIdException() : base("Invalid policy id.") { }
    }
}
