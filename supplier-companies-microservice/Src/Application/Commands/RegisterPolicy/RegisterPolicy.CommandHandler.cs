using Application.Core;

namespace SupplierCompany.Application
{
    using SupplierCompany.Domain;
    public class RegisterPolicyCommandHandler(IdService<string> idService, IMessageBrokerService messageBrokerService, IEventStore eventStore, ISupplierCompanyRepository supplierCompanyRepository) : IService<RegisterPolicyCommand, RegisterPolicyResponse>
    {
        private readonly IdService<string> _idService = idService;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly ISupplierCompanyRepository _supplierCompanyRepository = supplierCompanyRepository;

        async public Task<Result<RegisterPolicyResponse>> Execute(RegisterPolicyCommand command)
        {
            var supplierCompanyRegistered = await _supplierCompanyRepository.FindById(command.SupplierCompanyId);
            if (!supplierCompanyRegistered.HasValue()) return Result<RegisterPolicyResponse>.MakeError(new SupplierCompanyNotFoundError());

            var supplierCompany = supplierCompanyRegistered.Unwrap();
            if (supplierCompany.GetPolicies().Any(policy => policy.GetTitle().GetValue().Equals(command.Title)))
            {
                return Result<RegisterPolicyResponse>.MakeError(new PolicyAlreadyExistsError(command.Title));
            }

            if (command.ExpirationDate < command.IssuanceDate) return Result<RegisterPolicyResponse>.MakeError(new InvalidPolicyExpirationDateError());

            var policyId = _idService.GenerateId();
            supplierCompany.RegisterPolicy(
                    new PolicyId(policyId),
                    new PolicyTitle(command.Title),
                    new PolicyCoverageAmount(command.CoverageAmount),
                    new PolicyCoverageDistance(command.CoverageDistance),
                    new PolicyPrice(command.Price),
                    new PolicyType(command.Type),
                    new PolicyIssuanceDate(command.IssuanceDate),
                    new PolicyExpirationDate(command.ExpirationDate)
                );

            var events = supplierCompany.PullEvents();
            await _supplierCompanyRepository.Save(supplierCompany);
            await _eventStore.AppendEvents(events);
            await _messageBrokerService.Publish(events);

            return Result<RegisterPolicyResponse>.MakeSuccess(new RegisterPolicyResponse(policyId));
        }
    }
}
