using Application.Core;

namespace SupplierCompany.Domain
{
    public record DepartmentListType(string Id, string Name, List<string> Employees);

    public class SupplierCompanyDepartmentsUpdatedEvent(string publisherId, string type, SupplierCompanyDepartmentsUpdated context) : DomainEvent(publisherId, type, context) { }

    public class SupplierCompanyDepartmentsUpdated(List<DepartmentListType> departments)
    {
        public readonly List<DepartmentListType> Departments = departments;
        public static SupplierCompanyDepartmentsUpdatedEvent CreateEvent(SupplierCompanyId publisherId, List<Department> departments)
        {
            return new SupplierCompanyDepartmentsUpdatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyDepartmentsUpdated).Name,
                new SupplierCompanyDepartmentsUpdated(
                    departments.Select(d => new DepartmentListType(
                        d.GetId().GetValue(),
                        d.GetName().GetValue(),
                        d.GetEmployees().Select(e =>
                            e.GetValue()).ToList())
                    ).ToList()
                )
            );
        }
    }
}
