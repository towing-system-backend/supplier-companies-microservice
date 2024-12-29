using Application.Core;
using Microsoft.AspNetCore.Mvc;
using SupplierCompany.Application;
using SupplierCompany.Domain;

namespace SupplierCompany.Infrastructure
{
    [ApiController]
    [Route("api/suppliercompany")]
    public class SupplierCompanyController(IdService<string> idService, Logger logger, IMessageBrokerService messageBrokerService, IEventStore eventStore, ISupplierCompanyRepository supplierCompanyRepository, IPerformanceLogsRepository performanceLogsRepository) : ControllerBase
    {

        private readonly IdService<string> _idService = idService;
        private readonly Logger _logger = logger;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly ISupplierCompanyRepository _supplierCompanyRepository = supplierCompanyRepository;
        private readonly IPerformanceLogsRepository _performanceLogsRepository = performanceLogsRepository;

        [HttpPost("create")]
        public async Task<ObjectResult> CreateSupplierCompany([FromBody] CreateSupplierCompanyDto createSupplierCompanyDto)
        {
            var command = new RegisterSupplierCompanyCommand(
                createSupplierCompanyDto.Departments.Select(d => new Application.Department(d.Name, d.Employee)).ToList(),
                createSupplierCompanyDto.Policies.Select(p => new Application.Policy(p.Title, p.CoverageAmount, p.Price, p.Type, p.IssuanceDate, p.ExpirationDate)).ToList(),
                createSupplierCompanyDto.TowDrivers,
                createSupplierCompanyDto.Name,
                createSupplierCompanyDto.PhoneNumber,
                createSupplierCompanyDto.Type,
                createSupplierCompanyDto.Rif,
                createSupplierCompanyDto.State,
                createSupplierCompanyDto.City,
                createSupplierCompanyDto.Street
            );
            var handler =
               new ExceptionCatcher<RegisterSupplierCompanyCommand, RegisterSupplierCompanyResponse>(
                   new PerfomanceMonitor<RegisterSupplierCompanyCommand, RegisterSupplierCompanyResponse>(
                       new LoggingAspect<RegisterSupplierCompanyCommand, RegisterSupplierCompanyResponse>(
                           new RegisterSupplierCompanyCommandHandler(_idService, _messageBrokerService, _eventStore, _supplierCompanyRepository), _logger
                       ), _logger, _performanceLogsRepository, nameof(RegisterSupplierCompanyCommandHandler), "Write"
                   ), ExceptionParser.Parse
               );
            var res = await handler.Execute(command);

            return Ok(res.Unwrap());
        }

        [HttpPost("update")]
        public async Task<ObjectResult> UpdateSupplierCompany([FromBody] UpdateSupplierCompanyDto udpateSupplierCompanyDto)
        {
            var command = new UpdateSupplierCompanyCommand(
                udpateSupplierCompanyDto.Id,
                udpateSupplierCompanyDto.Departments.Select(d => new Application.Department(d.Name, d.Employee)).ToList(),
                udpateSupplierCompanyDto.Policies.Select(p => new Application.Policy(p.Title, p.CoverageAmount, p.Price, p.Type, p.IssuanceDate, p.ExpirationDate)).ToList(),
                udpateSupplierCompanyDto.TowDrivers,
                udpateSupplierCompanyDto.Name,
                udpateSupplierCompanyDto.PhoneNumber,
                udpateSupplierCompanyDto.Type,
                udpateSupplierCompanyDto.Rif,
                udpateSupplierCompanyDto.State,
                udpateSupplierCompanyDto.City,
                udpateSupplierCompanyDto.Street
            );
            var handler =
               new ExceptionCatcher<UpdateSupplierCompanyCommand, UpdateSupplierCompanyResponse>(
                   new PerfomanceMonitor<UpdateSupplierCompanyCommand, UpdateSupplierCompanyResponse>(
                       new LoggingAspect<UpdateSupplierCompanyCommand, UpdateSupplierCompanyResponse>(
                           new UpdateSupplierCompanyCommandHandler(_idService, _messageBrokerService, _eventStore, _supplierCompanyRepository), _logger
                       ), _logger, _performanceLogsRepository, nameof(UpdateSupplierCompanyCommandHandler), "Write"
                   ), ExceptionParser.Parse
               );
            var res = await handler.Execute(command);

            return Ok(res.Unwrap());
        }

        [HttpPost("create/department")]
        public async Task<ObjectResult> CreateDepartment([FromBody] CreateDepartmentDto createDepartmentDto)
        {
            var command = new RegisterDepartmentCommand(
                createDepartmentDto.SupplierCompanyId,
                createDepartmentDto.Name,
                createDepartmentDto.Employees
            );
            var handler =
                new ExceptionCatcher<RegisterDepartmentCommand, RegisterDepartmentResponse>(
                    new PerfomanceMonitor<RegisterDepartmentCommand, RegisterDepartmentResponse>(
                        new LoggingAspect<RegisterDepartmentCommand, RegisterDepartmentResponse>(
                            new RegisterDepartmentCommandHandler(_idService, _messageBrokerService, _eventStore, _supplierCompanyRepository), _logger
                        ), _logger, _performanceLogsRepository, nameof(RegisterDepartmentCommandHandler), "Write"
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);

            return Ok(res.Unwrap());
        }

        [HttpPost("create/policy")]
        public async Task<ObjectResult> CreatePolicy([FromBody] CreatePolicyDto createPolicyDto)
        {
            var command = new RegisterPolicyCommand(
                createPolicyDto.SupplierCompanyId,
                createPolicyDto.Title,
                createPolicyDto.CoverageAmount,
                createPolicyDto.Price,
                createPolicyDto.Type,
                createPolicyDto.IssuanceDate,
                createPolicyDto.ExpirationDate
            );

            var handler = new ExceptionCatcher<RegisterPolicyCommand, RegisterPolicyResponse>(
                new PerfomanceMonitor<RegisterPolicyCommand, RegisterPolicyResponse>(
                    new LoggingAspect<RegisterPolicyCommand, RegisterPolicyResponse>(
                        new RegisterPolicyCommandHandler(_idService, _messageBrokerService, _eventStore, _supplierCompanyRepository), _logger
                    ), _logger, _performanceLogsRepository, nameof(RegisterPolicyCommandHandler), "Write"
                ), ExceptionParser.Parse
            );
            var res = await handler.Execute(command);

            return Ok(res.Unwrap());
        }

        [HttpPost("register/towdriver")]
        public async Task<ObjectResult> RegisterTowDriver([FromBody] RegisterTowDriverDto registerTowDriverDto)
        {
            var command = new RegisterTowDriverCommand(
                registerTowDriverDto.SupplierCompanyId,
                registerTowDriverDto.Id
            );

            var handler = new ExceptionCatcher<RegisterTowDriverCommand, RegisterTowDriverResponse>(
                new PerfomanceMonitor<RegisterTowDriverCommand, RegisterTowDriverResponse>(
                    new LoggingAspect<RegisterTowDriverCommand, RegisterTowDriverResponse>(
                        new RegisterTowDriverCommandHandler(_messageBrokerService, _eventStore, _supplierCompanyRepository), _logger
                    ), _logger, _performanceLogsRepository, nameof(RegisterTowDriverCommandHandler), "Write"
                ), ExceptionParser.Parse
            );
            var res = await handler.Execute(command);

            return Ok(res.Unwrap());
        }
    }
}
