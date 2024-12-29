using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidUserIdException : DomainException
    {
        public InvalidUserIdException() : base("Invalid user id.") { }
    }
}
