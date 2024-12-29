using Application.Core;

namespace SupplierCompany.Application
{
    public class DuplicateEmployeeError : ApplicationError
    {
        public DuplicateEmployeeError() : base("Some department has a duplicate employee.") { }
    }
}
