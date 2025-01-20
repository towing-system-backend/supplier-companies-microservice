using Application.Core;

namespace SupplierCompany.Application
{
    using SupplierCompany.Domain;
    public class UpdateSupplierCompanyCommandHandler(IdService<string> idService, IMessageBrokerService messageBrokerService, IEventStore eventStore, ISupplierCompanyRepository supplierCompanyRepository) : IService<UpdateSupplierCompanyCommand, UpdateSupplierCompanyResponse>
    {
        private readonly IdService<string> _idService = idService;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly ISupplierCompanyRepository _supplierCompanyRepository = supplierCompanyRepository;

        async public Task<Result<UpdateSupplierCompanyResponse>> Execute(UpdateSupplierCompanyCommand command)
        {
            var supplierCompanyRegistered = await _supplierCompanyRepository.FindById(command.Id);
            if (!supplierCompanyRegistered.HasValue()) return Result<UpdateSupplierCompanyResponse>.MakeError(new SupplierCompanyNotFoundError());

            var supplierCompany = supplierCompanyRegistered.Unwrap();

            if (command.Departments != null)
            {
                foreach (var department in command.Departments)
                {
                    var existingDepartment = supplierCompany.GetDepartments().First(d => d.GetId().GetValue() == department.Id);
                    
                    if (!string.IsNullOrEmpty(department.Name))
                    {
                        existingDepartment.SetName(new DepartmentName(department.Name));
                    }

                    if (department.Employees != null)
                    {
                        if (department.Employees.Distinct().Count() != department.Employees.Count)
                        {
                            return Result<UpdateSupplierCompanyResponse>.MakeError(new DuplicateEmployeeError());
                        }

                        existingDepartment.SetEmployees(department.Employees.Select(e => new UserId(e)).ToList());
                    }
                }

                supplierCompany.UpdateSupplierCompanyDepartments(supplierCompany.GetDepartments());
            }

            if (command.Policies != null)
            {
                if (command.Policies.Any(p => p.ExpirationDate < p.IssuanceDate)) return Result<UpdateSupplierCompanyResponse>.MakeError(new InvalidPolicyExpirationDateError());
                
                var policies = command.Policies.Select(p =>
                        new Domain.Policy(
                        new PolicyId(_idService.GenerateId()),
                        new PolicyTitle(p.Title),
                        new PolicyCoverageAmount(p.CoverageAmount),
                        new PolicyCoverageDistance(p.CoverageDistance),
                        new PolicyPrice(p.Price),
                        new PolicyType(p.Type),
                        new PolicyIssuanceDate(p.IssuanceDate),
                        new PolicyExpirationDate(p.ExpirationDate)
                    )
                ).ToList();

                supplierCompany.UpdateSupplierCompanyPolicies(policies);
            }

            if (command.TowDrivers != null)
            {
                if (command.TowDrivers.Distinct().Count() != command.TowDrivers.Count) return Result<UpdateSupplierCompanyResponse>.MakeError(new DuplicateTowDriverError());
                var towDrivers = command.TowDrivers.Select(t => new TowDriverId(t)).ToList();

                supplierCompany.UpdateSupplierCompanyTowDrivers(towDrivers);
            }

            if (command.Name != null) supplierCompany.UpdateSupplierCompanyName(new SupplierCompanyName(command.Name));
            if (command.PhoneNumber != null) supplierCompany.UpdateSupplierCompanyPhoneNumber(new SupplierCompanyPhoneNumber(command.PhoneNumber));
            if (command.Type != null) supplierCompany.UpdateSupplierCompanyType(new SupplierCompanyType(command.Type));
            if (command.Rif != null) supplierCompany.UpdateSupplierCompanyRif(new SupplierCompanyRif(command.Rif));
            if (command.State != null && command.City != null && command.Street != null) supplierCompany.UpdateSupplierCompanyAddress(new SupplierCompanyAddress(command.State, command.City, command.Street));


            var events = supplierCompany.PullEvents();
            await _supplierCompanyRepository.Save(supplierCompany);
            await _eventStore.AppendEvents(events);
            await _messageBrokerService.Publish(events);

            return Result<UpdateSupplierCompanyResponse>.MakeSuccess(new UpdateSupplierCompanyResponse(command.Id));
        }
    }
}