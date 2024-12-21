using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidSupplierCompanyPhoneNumberException : DomainException
    {
        public InvalidSupplierCompanyPhoneNumberException() : base("Invalid supplier company phone number.") { }
    }
}
