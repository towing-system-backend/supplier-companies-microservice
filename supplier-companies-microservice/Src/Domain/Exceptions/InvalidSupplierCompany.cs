using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidSupplierCompanyException : DomainException
    {
        public InvalidSupplierCompanyException() : base("Invalid supplier company.") { }
    }
}
