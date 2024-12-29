using Application.Core;

namespace SupplierCompany.Application
{
    public class InvalidEmployeeError : ApplicationError
    {
        public InvalidEmployeeError() : base("The employee id must be Guid.") { }
    }
}
