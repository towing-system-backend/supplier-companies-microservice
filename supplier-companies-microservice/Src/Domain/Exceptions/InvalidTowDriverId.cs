using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidTowDriverIdException : DomainException
    {
        public InvalidTowDriverIdException() : base("Invalid tow driver id.") { }
    }
}
