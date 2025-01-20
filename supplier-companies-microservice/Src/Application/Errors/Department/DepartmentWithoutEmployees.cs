using Application.Core;

namespace SupplierCompany.Application
{
    public class DepartmentWithoutEmployeesError : ApplicationError
    {
        public DepartmentWithoutEmployeesError() : base("Departments must have at least one employee.") { }
    }
}