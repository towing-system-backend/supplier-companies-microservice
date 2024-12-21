using Application.Core;

namespace SupplierCompany.Application
{
    using SupplierCompany.Domain;
    public class RegisterSupplierCompanyCommandHandler(IdService<string> idService, IMessageBrokerService messageBrokerService, IEventStore eventStore, ISupplierCompanyRepository supplierCompanyRepository) : IService<RegisterSupplierCompanyCommand, RegisterSupplierCompanyResponse>
    {
        private readonly IdService<string> _idService = idService;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly ISupplierCompanyRepository _supplierCompanyRepository = supplierCompanyRepository;
        async public Task<Result<RegisterSupplierCompanyResponse>> Execute(RegisterSupplierCompanyCommand command)
        {
            var rifRegistered = await _supplierCompanyRepository.FindByRif(command.Rif);
            if (rifRegistered.HasValue()) return Result<RegisterSupplierCompanyResponse>.MakeError(new SupplierCompanyRegisteredError(command.Rif));

            var departments = command.Departments.Select(d =>
                new Domain.Department(
                    new DepartmentId(_idService.GenerateId()),
                    new DepartmentName(d.Name),
                    d.Employees.Select(e => new UserId(e)).ToList()
                )
            ).ToList();

            var policies = command.Policies.Select(p =>
                new Domain.Policy(
                    new PolicyId(_idService.GenerateId()),
                    new PolicyTitle(p.Title),
                    new PolicyCoverageAmount(p.CoverageAmount),
                    new PolicyPrice(p.Price),
                    new PolicyType(p.Type),
                    new PolicyIssuanceDate(p.IssuanceDate),
                    new PolicyExpirationDate(p.ExpirationDate)
                )
            ).ToList();

            var towDrivers = command.TowDrivers.Select(t => new TowDriverId(t)).ToList();

            var supplierCompanyId = _idService.GenerateId();
            var supplierCompany = SupplierCompany.Create(
                new SupplierCompanyId(supplierCompanyId),
                departments,
                policies,
                towDrivers,
                new SupplierCompanyName(command.Name),
                new SupplierCompanyPhoneNumber(command.PhoneNumber),
                new SupplierCompanyType(command.Type),
                new SupplierCompanyRif(command.Rif),
                new SupplierCompanyAddress(command.State, command.City, command.Street)
            );

            var events = supplierCompany.PullEvents();
            await _supplierCompanyRepository.Save(supplierCompany);
            await _eventStore.AppendEvents(events);
            await _messageBrokerService.Publish(events);

            return Result<RegisterSupplierCompanyResponse>.MakeSuccess(new RegisterSupplierCompanyResponse(supplierCompanyId));
        }
    }
}