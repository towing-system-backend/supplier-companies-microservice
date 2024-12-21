using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidSupplierCompanyIdException : DomainException
    {
        public InvalidSupplierCompanyIdException() : base("Invalid supplier company id.") { }
    }
}
