using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidSupplierCompanyAddressException : DomainException
    {
        public InvalidSupplierCompanyAddressException() : base("Invalid supplier company address.") { }
    }
}
