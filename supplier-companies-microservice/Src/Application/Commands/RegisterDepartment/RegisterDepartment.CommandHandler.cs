using Application.Core;

namespace SupplierCompany.Application
{
    using SupplierCompany.Domain;
    public class RegisterDepartmentCommandHandler(IdService<string> idService, IMessageBrokerService messageBrokerService, IEventStore eventStore, ISupplierCompanyRepository supplierCompanyRepository) : IService<RegisterDepartmentCommand, RegisterDepartmentResponse>
    {
        private readonly IdService<string> _idService = idService;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly ISupplierCompanyRepository _supplierCompanyRepository = supplierCompanyRepository;

        async public Task<Result<RegisterDepartmentResponse>> Execute(RegisterDepartmentCommand command)
        {
            var supplierCompanyRegistered = await _supplierCompanyRepository.FindById(command.SupplierCompanyId);
            if (!supplierCompanyRegistered.HasValue()) return Result<RegisterDepartmentResponse>.MakeError(new SupplierCompanyNotFoundError());

            var supplierCompany = supplierCompanyRegistered.Unwrap();
            if (supplierCompany.GetDepartments().Any(department => department.GetName().GetValue().Equals(command.Name)))
            {
               return Result<RegisterDepartmentResponse>.MakeError(new DepartmentAlreadyExistsError(command.Name));
            }

            if (command.Employees.Any(e => !GuidEx.IsGuid(e))) return Result<RegisterDepartmentResponse>.MakeError(new InvalidEmployeeError());
            if (command.Employees.Distinct().Count() != command.Employees.Count) return Result<RegisterDepartmentResponse>.MakeError(new DuplicateEmployeeError());

            var departmentId = _idService.GenerateId();
            supplierCompany.RegisterDepartment(
                new DepartmentId(departmentId),
                new DepartmentName(command.Name),
                command.Employees.Select(employee => new UserId(employee)).ToList()
            );

            var events = supplierCompany.PullEvents();
            await _supplierCompanyRepository.Save(supplierCompany);
            await _eventStore.AppendEvents(events);
            await _messageBrokerService.Publish(events);

            return Result<RegisterDepartmentResponse>.MakeSuccess(new RegisterDepartmentResponse(departmentId));
        }
    }
}
