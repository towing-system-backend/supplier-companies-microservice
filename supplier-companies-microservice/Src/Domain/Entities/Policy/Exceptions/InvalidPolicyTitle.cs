using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidPolicyTitleException : DomainException
    {
        public InvalidPolicyTitleException() : base("Invalid policy title.") { }
    }
}
