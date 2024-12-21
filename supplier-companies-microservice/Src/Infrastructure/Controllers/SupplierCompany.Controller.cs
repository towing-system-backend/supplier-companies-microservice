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
        public async Task<ObjectResult> CreateUser([FromBody] CreateSupplierCompanyDto createSupplierCompanyDto)
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
                       ), _logger, _performanceLogsRepository, "RegisterSupplierCompanyCommandHandler", "Write"
                   ), ExceptionParser.Parse
               );
            var res = await handler.Execute(command);

            return Ok(res.Unwrap());
        }
    }
}
