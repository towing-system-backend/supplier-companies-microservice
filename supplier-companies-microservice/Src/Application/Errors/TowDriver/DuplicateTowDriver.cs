using Application.Core;

namespace SupplierCompany.Application
{
    public class DuplicateTowDriverError : ApplicationError
    {
        public DuplicateTowDriverError() : base("Some tow driver have registered twice.") { }
    }
}
