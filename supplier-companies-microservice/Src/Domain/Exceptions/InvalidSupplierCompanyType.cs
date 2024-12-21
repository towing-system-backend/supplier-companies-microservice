using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidSupplierCompanyTypeException : DomainException
    {
        public InvalidSupplierCompanyTypeException() : base("Invalid supplier company type.") { }
    }
}
