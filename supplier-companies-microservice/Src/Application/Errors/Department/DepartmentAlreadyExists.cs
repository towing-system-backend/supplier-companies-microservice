using Application.Core;

namespace SupplierCompany.Application
{
    public class DepartmentAlreadyExistsError : ApplicationError
    {
        public DepartmentAlreadyExistsError(string name) : base($"Department with name '{name}' already exists.") { }
    }
}
