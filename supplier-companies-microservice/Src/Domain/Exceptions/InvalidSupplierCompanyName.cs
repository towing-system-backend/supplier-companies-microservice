using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidSupplierCompanyNameException : DomainException
    {
        public InvalidSupplierCompanyNameException() : base("Invalid supplier company name.") { }
    }
}
