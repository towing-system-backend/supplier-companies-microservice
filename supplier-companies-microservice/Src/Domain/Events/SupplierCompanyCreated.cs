using Application.Core;

namespace SupplierCompany.Domain
{
    public record DepartmentInfo(string Id, string Name, List<string> Employees);
    public record EmployeeInfo(string Id);
    public record PolicyInfo(string Id, string Title, int CoverageAmount, int CoverageDistance, decimal Price, string Type, DateOnly IssuanceDate, DateOnly ExpirationDate);
    public class SupplierCompanyCreatedEvent(string publisherId, string type, SupplierCompanyCreated context) : DomainEvent(publisherId, type, context) { }

    public class SupplierCompanyCreated(
        string publisherId,
        List<DepartmentInfo> departments,
        List<PolicyInfo> policies,
        List<string> towDrivers,
        string name,
        string phoneNumber,
        string type,
        string rif,
        string state,
        string city,
        string street)
    {
        public readonly string PublisherId = publisherId;
        public readonly List<DepartmentInfo> Departments = departments;
        public readonly List<PolicyInfo> Policies = policies;
        public readonly List<string> TowDrivers = towDrivers;
        public readonly string Name = name;
        public readonly string PhoneNumber = phoneNumber;
        public readonly string Type = type;
        public readonly string Rif = rif;
        public readonly string State = state;
        public readonly string City = city;
        public readonly string Street = street;

        public static SupplierCompanyCreatedEvent CreateEvent(
            SupplierCompanyId publisherId,
            List<Department> departments,
            List<Policy> policies,
            List<TowDriverId> towDrivers,
            SupplierCompanyName name,
            SupplierCompanyPhoneNumber phoneNumber,
            SupplierCompanyType type,
            SupplierCompanyRif rif,
            SupplierCompanyAddress address)
        {
            var departmentInfos = departments.Select(d => new DepartmentInfo(
                d.GetId().GetValue(),
                d.GetName().GetValue(),
                d.GetEmployees().Select(e => e.GetValue()).ToList()
            )).ToList();

            var policyInfos = policies.Select(p => new PolicyInfo(
                p.GetId().GetValue(),
                p.GetTitle().GetValue(),
                p.GetCoverageAmount().GetValue(),
                p.GetCoverageDistance().GetValue(),
                p.GetPrice().GetValue(),
                p.GetType().GetValue(),
                p.GetIssuanceDate().GetValue(),
                p.GetExpirationDate().GetValue()
            )).ToList();

            var towDriverIds = towDrivers.Select(t => t.GetValue()).ToList();

            return new SupplierCompanyCreatedEvent(
                publisherId.GetValue(),
                typeof(SupplierCompanyCreated).Name,
                new SupplierCompanyCreated(
                    publisherId.GetValue(),
                    departmentInfos,
                    policyInfos,
                    towDriverIds,
                    name.GetValue(),
                    phoneNumber.GetValue(),
                    type.GetValue(),
                    rif.GetValue(),
                    address.GetState(),
                    address.GetCity(),
                    address.GetStreet()
                )
            );
        }
    }
}