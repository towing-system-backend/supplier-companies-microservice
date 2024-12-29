using Application.Core;

namespace SupplierCompany.Application
{
    public class DepartmentAlreadyExistsError : ApplicationError
    {
        public DepartmentAlreadyExistsError(string departmentName) : base($"Department with name {departmentName} already exists.") { }
    }
}
