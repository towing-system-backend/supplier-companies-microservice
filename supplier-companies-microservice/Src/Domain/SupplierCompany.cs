using Application.Core;

namespace SupplierCompany.Domain
{
    public class SupplierCompany : AggregateRoot<SupplierCompanyId>
    {
        private new SupplierCompanyId Id;
        private List<Department> _departments;
        private List<Policy> _policies;
        private List<TowDriverId> _towDrivers;
        private SupplierCompanyName _name;
        private SupplierCompanyPhoneNumber _phoneNumber;
        private SupplierCompanyType _type;
        private SupplierCompanyRif _rif;
        private SupplierCompanyAddress _address;

        private SupplierCompany(SupplierCompanyId supplierCompanyId) : base(supplierCompanyId)
        {
            Id = supplierCompanyId;
        }

        public override void ValidateState()
        {
            if (Id == null ||
                _departments.Count == 0 ||
                _departments.Select(d => d.GetName().GetValue()).Distinct().Count() != _departments.Count ||
                _policies.Count == 0 ||
                _policies.Select(p => p.GetTitle().GetValue()).Distinct().Count() != _policies.Count ||
                _towDrivers.Count == 0 ||
                _policies.Select(p => p.GetTitle().GetValue()).Distinct().Count() != _policies.Count ||
                _policies.Any(p => p.GetExpirationDate().GetValue() < p.GetIssuanceDate().GetValue()) ||
                _name == null ||
                _phoneNumber == null ||
                _type == null ||
                _rif == null ||
                _address == null)
            {
                throw new InvalidSupplierCompanyException();
            }
        }

        public SupplierCompanyId GetSupplierCompanyId() => Id;
        public List<Department> GetDepartments() => _departments;
        public List<Policy> GetPolicies() => _policies;
        public List<TowDriverId> GetTowDrivers() => _towDrivers;
        public SupplierCompanyName GetName() => _name;
        public SupplierCompanyPhoneNumber GetPhoneNumber() => _phoneNumber;
        public SupplierCompanyType GetType() => _type;
        public SupplierCompanyRif GetRif() => _rif;
        public SupplierCompanyAddress GetAddress() => _address;

        public static SupplierCompany Create(
            SupplierCompanyId supplierCompanyId,
            List<Department> departments,
            List<Policy> policies,
            List<TowDriverId> towDrivers,
            SupplierCompanyName name,
            SupplierCompanyPhoneNumber phoneNumber,
            SupplierCompanyType type,
            SupplierCompanyRif rif,
            SupplierCompanyAddress address,
            bool fromPersistence = false)
        {
            if (fromPersistence)
            {
                return new SupplierCompany(supplierCompanyId)
                {
                    Id = supplierCompanyId,
                    _departments = departments,
                    _policies = policies,
                    _towDrivers = towDrivers,
                    _name = name,
                    _phoneNumber = phoneNumber,
                    _type = type,
                    _rif = rif,
                    _address = address
                };
            }

            var supplierCompany = new SupplierCompany(supplierCompanyId);
            supplierCompany.Apply(
                SupplierCompanyCreated.CreateEvent(
                    supplierCompanyId,
                    departments,
                    policies,
                    towDrivers,
                    name,
                    phoneNumber,
                    type,
                    rif,
                    address
                )
            );

            return supplierCompany;
        }

        public void RegisterDepartment(DepartmentId id, DepartmentName name, List<UserId> employees)
        {
            Apply(DepartmentRegistered.CreateEvent(Id, id, name, employees));
        }

        public void RegisterPolicy(
            PolicyId id,
            PolicyTitle title,
            PolicyCoverageAmount coverageAmount,
            PolicyCoverageDistance coverageDistance,
            PolicyPrice price,
            PolicyType type,
            PolicyIssuanceDate issuanceDate,
            PolicyExpirationDate expirationDate
        )
        {
            Apply(PolicyRegistered.CreateEvent(Id, id, title, coverageAmount, coverageDistance, price, type, issuanceDate, expirationDate));
        }

        public void RegisterTowDriver(UserId id)
        {
            Apply(TowDriverRegistered.CreateEvent(Id, id));
        }

        public void UpdateSupplierCompanyDepartments(List<Department> departments)
        {
            Apply(SupplierCompanyDepartmentsUpdated.CreateEvent(Id, departments));
        }

        public void UpdateSupplierCompanyPolicies(List<Policy> policies)
        {
            Apply(SupplierCompanyPoliciesUpdated.CreateEvent(Id, policies));
        }

        public void UpdateSupplierCompanyTowDrivers(List<TowDriverId> towDrivers)
        {
            Apply(SupplierCompanyTowDriversUpdated.CreateEvent(Id, towDrivers));
        }

        public void UpdateSupplierCompanyName(SupplierCompanyName name)
        {
            Apply(SupplierCompanyNameUpdated.CreateEvent(Id, name));
        }

        public void UpdateSupplierCompanyPhoneNumber(SupplierCompanyPhoneNumber phoneNumber)
        {
            Apply(SupplierCompanyPhoneNumberUpdated.CreateEvent(Id, phoneNumber));
        }

        public void UpdateSupplierCompanyType(SupplierCompanyType type)
        {
            Apply(SupplierCompanyTypeUpdated.CreateEvent(Id, type));
        }

        public void UpdateSupplierCompanyRif(SupplierCompanyRif rif)
        {
            Apply(SupplierCompanyRifUpdated.CreateEvent(Id, rif));
        }

        public void UpdateSupplierCompanyAddress(SupplierCompanyAddress address)
        {
            Apply(SupplierCompanyAddressUpdated.CreateEvent(Id, address));
        }
       
        private void OnSupplierCompanyCreatedEvent(SupplierCompanyCreated context)
        {
            _departments = context.Departments.Select(d => 
                new Department(
                    new DepartmentId(d.Id),
                    new DepartmentName(d.Name),
                    d.Employees.Select(e => new UserId(e)).ToList()
                )
            ).ToList();

            _policies = context.Policies.Select(p => 
                new Policy(
                    new PolicyId(p.Id),
                    new PolicyTitle(p.Title),
                    new PolicyCoverageAmount(p.CoverageAmount),
                    new PolicyCoverageDistance(100),
                    new PolicyPrice(p.Price),
                    new PolicyType(p.Type),
                    new PolicyIssuanceDate(p.IssuanceDate),
                    new PolicyExpirationDate(p.ExpirationDate)
                )
            ).ToList();

            _towDrivers = context.TowDrivers.Select(t => new TowDriverId(t)).ToList();
            _name = new SupplierCompanyName(context.Name);
            _phoneNumber = new SupplierCompanyPhoneNumber(context.PhoneNumber);
            _type = new SupplierCompanyType(context.Type);
            _rif = new SupplierCompanyRif(context.Rif);
            _address = new SupplierCompanyAddress(context.State, context.City, context.Street);
        }
       
        private void OnDepartmentRegisteredEvent(DepartmentRegistered context)
        {
            _departments.Add(
                new Department(
                    new DepartmentId(context.DepartmentId),
                    new DepartmentName(context.Name),
                    context.Employees.Select(e => new UserId(e)).ToList()
                )
            );
        }

        private void OnPolicyRegisteredEvent(PolicyRegistered context)
        {
            _policies.Add(
                new Policy(
                    new PolicyId(context.PolicyId),
                    new PolicyTitle(context.Title),
                    new PolicyCoverageAmount(context.CoverageAmount),
                    new PolicyCoverageDistance(context.CoverageDistance),
                    new PolicyPrice(context.Price),
                    new PolicyType(context.Type),
                    new PolicyIssuanceDate(context.IssuanceDate),
                    new PolicyExpirationDate(context.ExpirationDate)
                )
            );
        }

        private void OnTowDriverRegisteredEvent(TowDriverRegistered context)
        {
            _towDrivers.Add(new TowDriverId(context.TowDriverId));
        }

        private void OnSupplierCompanyDepartmentsUpdatedEvent(SupplierCompanyDepartmentsUpdated context)
        {
            _departments = context.Departments.Select(d => 
                new Department(
                    new DepartmentId(d.Id),
                    new DepartmentName(d.Name),
                    d.Employees.Select(e => new UserId(e)).ToList()
                )
            ).ToList();
        }

        private void OnSupplierCompanyPoliciesUpdatedEvent(SupplierCompanyPoliciesUpdated context)
        {
            _policies = context.Policies.Select(p => 
                new Policy(
                    new PolicyId(p.Id),
                    new PolicyTitle(p.Title),
                    new PolicyCoverageAmount(p.CoverageAmount),
                    new PolicyCoverageDistance(p.CoverageDistance),
                    new PolicyPrice(p.Price),
                    new PolicyType(p.Type),
                    new PolicyIssuanceDate(p.IssuanceDate),
                    new PolicyExpirationDate(p.ExpirationDate)
                )
            ).ToList();
        }

        private void OnSupplierCompanyTowDriversUpdatedEvent(SupplierCompanyTowDriversUpdated context)
        {
            _towDrivers = context.TowDrivers.Select(t => new TowDriverId(t)).ToList();
        }   

        private void OnSupplierCompanyNameUpdatedEvent(SupplierCompanyNameUpdated context)
        {
            _name = new SupplierCompanyName(context.Name);
        }

        private void OnSupplierCompanyPhoneNumberUpdatedEvent(SupplierCompanyPhoneNumberUpdated context)
        {
            _phoneNumber = new SupplierCompanyPhoneNumber(context.PhoneNumber);
        }

        private void OnSupplierCompanyTypeUpdatedEvent(SupplierCompanyTypeUpdated context)
        {
            _type = new SupplierCompanyType(context.Type);
        }

        private void OnSupplierCompanyRifUpdatedEvent(SupplierCompanyRifUpdated context)
        {
            _rif = new SupplierCompanyRif(context.Rif);
        }

        private void OnSupplierCompanyAddressUpdatedEvent(SupplierCompanyAddressUpdated context)
        {
            _address = new SupplierCompanyAddress(context.State, context.City, context.Street);
        }
    }
}