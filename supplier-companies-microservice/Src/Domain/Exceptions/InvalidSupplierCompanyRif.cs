using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidSupplierCompanyRifException : DomainException
    {
        public InvalidSupplierCompanyRifException() : base("Invalid supplier company rif.") { }
    }
}
