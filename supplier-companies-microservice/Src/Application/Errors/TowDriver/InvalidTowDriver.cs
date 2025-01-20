using Application.Core;

namespace SupplierCompany.Application
{
    public class InvalidTowDriverError : ApplicationError
    {
        public InvalidTowDriverError() : base("The tow driver id must be Guid.") { }
    }
}
