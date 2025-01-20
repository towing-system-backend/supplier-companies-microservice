using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidDepartmentIdException : DomainException
    {
        public InvalidDepartmentIdException() : base("Invalid department id.") { }
    }
}
