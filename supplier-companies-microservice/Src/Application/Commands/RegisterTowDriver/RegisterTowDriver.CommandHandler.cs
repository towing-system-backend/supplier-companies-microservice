using Application.Core;

namespace SupplierCompany.Application
{
    using SupplierCompany.Domain;
    public class RegisterTowDriverCommandHandler(IMessageBrokerService messageBrokerService, IEventStore eventStore, ISupplierCompanyRepository supplierCompanyRepository) : IService<RegisterTowDriverCommand, RegisterTowDriverResponse>
    {
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly ISupplierCompanyRepository _supplierCompanyRepository = supplierCompanyRepository;

        async public Task<Result<RegisterTowDriverResponse>> Execute(RegisterTowDriverCommand command)
        {
            var supplierCompanyRegistered = await _supplierCompanyRepository.FindById(command.SupplierCompanyId);
            if (!supplierCompanyRegistered.HasValue()) return Result<RegisterTowDriverResponse>.MakeError(new SupplierCompanyNotFoundError());

            var supplierCompany = supplierCompanyRegistered.Unwrap();

            if (!GuidEx.IsGuid(command.Id)) return Result<RegisterTowDriverResponse>.MakeError(new InvalidTowDriverError());
            if (supplierCompany.GetTowDrivers().Any(towDriver => towDriver.GetValue().Equals(command.Id)))
            {
                return Result<RegisterTowDriverResponse>.MakeError(new TowDriverAlreadyExistsError(command.Id));
            }

            supplierCompany.RegisterTowDriver(
                new UserId(command.Id)
            );

            var events = supplierCompany.PullEvents();
            await _supplierCompanyRepository.Save(supplierCompany);
            await _eventStore.AppendEvents(events);
            await _messageBrokerService.Publish(events);

            return Result<RegisterTowDriverResponse>.MakeSuccess(new RegisterTowDriverResponse(command.Id));
        }
    }
}