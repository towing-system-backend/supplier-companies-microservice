using Application.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SupplierCompany.Application;
using SupplierCompany.Domain;

namespace SupplierCompany.Infrastructure
{
    [ApiController]
    [Route("api/suppliercompany")]
    public class SupplierCompanyController(
        IdService<string> idService,
        Logger logger,
        IMessageBrokerService
        messageBrokerService,
        IEventStore eventStore,
        ISupplierCompanyRepository supplierCompanyRepository,
        IPerformanceLogsRepository performanceLogsRepository
    ) : ControllerBase
    {
        private readonly IdService<string> _idService = idService;
        private readonly Logger _logger = logger;
        private readonly IMessageBrokerService _messageBrokerService = messageBrokerService;
        private readonly IEventStore _eventStore = eventStore;
        private readonly ISupplierCompanyRepository _supplierCompanyRepository = supplierCompanyRepository;
        private readonly IPerformanceLogsRepository _performanceLogsRepository = performanceLogsRepository;

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ObjectResult> CreateSupplierCompany([FromBody] CreateSupplierCompanyDto createSupplierCompanyDto)
        {
            var command = new RegisterSupplierCompanyCommand(
                createSupplierCompanyDto.Departments.Select(d => new Application.Department(d.Name)).ToList(),
                createSupplierCompanyDto.Policies.Select(p => new Application.Policy(p.Title, p.CoverageAmount, p.CoverageDistance, p.Price, p.Type, p.IssuanceDate, p.ExpirationDate)).ToList(),
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

        [HttpPatch("update")]
        [Authorize(Roles = "Admin")]
        public async Task<ObjectResult> UpdateSupplierCompany([FromBody] UpdateSupplierCompanyDto updateSupplierCompanyDto)
        {
            var departments = new List<DepartmentUpdate>();
            var policies = new List<PolicyUpdate>();

            if(updateSupplierCompanyDto.Departments != null)
            {
                departments = updateSupplierCompanyDto.Departments.Select(d => new DepartmentUpdate(d.DepartmentId, d.Name, d.Employees)).ToList();
            }

            if(updateSupplierCompanyDto.Policies != null)
            {
                policies = updateSupplierCompanyDto.Policies.Select(p => 
                new PolicyUpdate(
                    p.PolicyId,
                    p.Title,
                    p.CoverageAmount,
                    p.CoverageDistance,
                    p.Price,
                    p.Type,
                    p.IssuanceDate,
                    p.ExpirationDate)
                ).ToList();
            }

            var command = new UpdateSupplierCompanyCommand(
                updateSupplierCompanyDto.SupplierCompanyId,
                departments.IsNullOrEmpty() ? null : departments,
                policies.IsNullOrEmpty()? null : policies,
                updateSupplierCompanyDto.TowDrivers,
                updateSupplierCompanyDto.Name,
                updateSupplierCompanyDto.PhoneNumber,
                updateSupplierCompanyDto.Type,
                updateSupplierCompanyDto.Rif,
                updateSupplierCompanyDto.State,
                updateSupplierCompanyDto.City,
                updateSupplierCompanyDto.Street
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<ObjectResult> CreatePolicy([FromBody] CreatePolicyDto createPolicyDto)
        {
            var command = new RegisterPolicyCommand(
                createPolicyDto.SupplierCompanyId,
                createPolicyDto.Title,
                createPolicyDto.CoverageAmount,
                createPolicyDto.CoverageDistance,
                createPolicyDto.Price,
                createPolicyDto.Type,
                createPolicyDto.IssuanceDate,
                createPolicyDto.ExpirationDate
            );

            var handler = 
                new ExceptionCatcher<RegisterPolicyCommand, RegisterPolicyResponse>(
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
        [Authorize(Roles = "Admin")]
        public async Task<ObjectResult> RegisterTowDriver([FromBody] RegisterTowDriverDto registerTowDriverDto)
        {
            var command = new RegisterTowDriverCommand(
                registerTowDriverDto.SupplierCompanyId,
                registerTowDriverDto.Id
            );

            var handler = 
                new ExceptionCatcher<RegisterTowDriverCommand, RegisterTowDriverResponse>(
                    new PerfomanceMonitor<RegisterTowDriverCommand, RegisterTowDriverResponse>(
                        new LoggingAspect<RegisterTowDriverCommand, RegisterTowDriverResponse>(
                            new RegisterTowDriverCommandHandler(_messageBrokerService, _eventStore, _supplierCompanyRepository), _logger
                        ), _logger, _performanceLogsRepository, nameof(RegisterTowDriverCommandHandler), "Write"
                    ), ExceptionParser.Parse
                );
            var res = await handler.Execute(command);

            return Ok(res.Unwrap());
        }

        [HttpGet("find/all/suppliercompanies")]
        [Authorize(Roles = "Admin")]
        public async Task<ObjectResult> GetAllSupplierCompanies()
        {
            var query =
                new ExceptionCatcher<string, List<FindAllSupplierCompanyResponse>>(
                    new PerfomanceMonitor<string, List<FindAllSupplierCompanyResponse>>(
                        new LoggingAspect<string, List<FindAllSupplierCompanyResponse>>(
                                new FindAllSupplierCompaniesQuery(), _logger
                            ), _logger, _performanceLogsRepository, nameof(FindAllSupplierCompaniesQuery), "Read"
                    ), ExceptionParser.Parse
                );
            var res = await query.Execute("");

            return Ok(res.Unwrap());
        }

        [HttpGet("find/employees/suppliercompany")]
        [Authorize(Roles = "Admin")]
        public async Task<ObjectResult> GetSupplierCompanyEmployees([FromBody] string supplierCompanyId)
        {
            var query = new FindSupplierCompanyEmployeesQuery();
            var res = await query.Execute(supplierCompanyId);
            return Ok(res.Unwrap());
        }

        [HttpGet("find/towdrivers/suppliercompany")]
        [Authorize(Roles = "Admin")]
        public async Task<ObjectResult> GetSupplierCompanyTowDrivers([FromBody] string supplierCompanyId)
        {
            var query = new FindSupplierCompanyTowDriversQuery();
            var res = await query.Execute(supplierCompanyId);
            return Ok(res.Unwrap());
        }
        
        [HttpGet("find/policies/suppliercompany/{id}")]
        [Authorize(Roles = "Admin, Provider")]
        public async Task<ObjectResult> GetPolicyBySupplierCompanyId(string id)
        {
            var query = new FindPolicyBySupplierCompanyQuery();
            var res = await query.Execute(id);
            return Ok(res.Unwrap());
        }
    }
}