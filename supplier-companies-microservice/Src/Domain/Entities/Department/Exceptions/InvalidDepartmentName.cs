using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidDepartmentNameException : DomainException
    {
        public InvalidDepartmentNameException() : base("Invalid department name.") { }
    }
}
