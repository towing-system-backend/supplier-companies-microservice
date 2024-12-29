using Application.Core;

namespace SupplierCompany.Domain
{
    public class DepartmentRegisteredEvent(string publisherId, string type, DepartmentRegistered context) : DomainEvent(publisherId, type, context) { }

    public class DepartmentRegistered(string id, string name, List<string> employees)
    {
        public readonly string Id = id;
        public readonly string Name = name;
        public readonly List<string> Employees = employees;

        public static DepartmentRegisteredEvent CreateEvent(SupplierCompanyId publisherId, DepartmentId departmentId, DepartmentName name, List<UserId> employees)
        {
            return new DepartmentRegisteredEvent(
            publisherId.GetValue(),
            typeof(DepartmentRegistered).Name,
            new DepartmentRegistered(
                  departmentId.GetValue(),
                  name.GetValue(),
                  employees.Select(e => e.GetValue()).ToList()
                )
            );
        }
    }
}