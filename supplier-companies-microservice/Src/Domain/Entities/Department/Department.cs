using Application.Core;

namespace SupplierCompany.Domain
{
    public class Department(DepartmentId id, DepartmentName name, List<UserId> employees) : Entity<DepartmentId>(id)
    {
        private readonly DepartmentId _id = id;
        private DepartmentName _name = name;
        private List<UserId> _employees = employees;

        public void SetName(DepartmentName name) => _name = name;

        public void SetEmployees(List<UserId> employees) => _employees = employees;

        public DepartmentId GetId() => _id;
        public DepartmentName GetName() => _name;
        public List<UserId> GetEmployees() => _employees;
    }
}