using Application.Core;
using Moq;
using SupplierCompany.Application;
using SupplierCompany.Domain;
using Xunit;

namespace SupplierCompany.Tests
{
    public class RegisterSupplierCompanyCommandHandlerTests
    {
        private readonly Mock<IdService<string>> _idServiceMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<ISupplierCompanyRepository> _supplierCompanyRepositoryMock;
        private readonly RegisterSupplierCompanyCommandHandler _registerSupplierCompanyCommandHandler;

        public RegisterSupplierCompanyCommandHandlerTests()
        {
            _idServiceMock = new Mock<IdService<string>>();
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _eventStoreMock = new Mock<IEventStore>();
            _supplierCompanyRepositoryMock = new Mock<ISupplierCompanyRepository>();
            _registerSupplierCompanyCommandHandler = new RegisterSupplierCompanyCommandHandler(
                _idServiceMock.Object,
                _messageBrokerServiceMock.Object,
                _eventStoreMock.Object,
                _supplierCompanyRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Register_Company_When_Rif_Already_Exists()
        {
            // Arrange
            var command = new RegisterSupplierCompanyCommand(
                [
                    new Application.Department("Contabilidad")
                ],
                [
                    new Application.Policy("Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddDays(2))
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "c3889b66-4281-44fd-b731-57d7906eb566"
                ],
                "Gruas Caracas",
                "04145241547",
                "External",
                "J-87654321-9",
                "Distrito Capital",
                "Caracas",
                "Calle Paris"
            );

            var supplierCompany = Domain.SupplierCompany.Create(
                new SupplierCompanyId("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b"),
                [new Domain.Department(
                    new DepartmentId("82219df5-e4a4-4ce6-95e8-42ac0af0a2a9"),
                    new DepartmentName("Finanzas"),
                    [new UserId("e525f54f-e032-4aad-aa01-995391abe173")])
                ],
                [new Domain.Policy(
                        new PolicyId("b68422ba-ebd0-45ea-9709-a53c64593d10"),
                        new PolicyTitle("Accidente vial"),
                        new PolicyCoverageAmount(100000),
                        new PolicyCoverageDistance(100),
                        new PolicyPrice(5000),
                        new PolicyType("Especial"),
                        new PolicyIssuanceDate(DateOnly.FromDateTime(DateTime.Now).AddDays(1)),
                        new PolicyExpirationDate(DateOnly.FromDateTime(DateTime.Now).AddYears(1))
                    )
                ],
                [new TowDriverId("34624b38-b69f-4872-a939-25cc9723dd5f")],
                new SupplierCompanyName("Gruas Caracas"),
                new SupplierCompanyPhoneNumber("04145241547"),
                new SupplierCompanyType("External"),
                new SupplierCompanyRif("J-87654321-9"),
                new SupplierCompanyAddress("Distrito Capital", "Caracas", "Calle Paris"),
                true
            );

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindByRif(command.Rif))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _registerSupplierCompanyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<SupplierCompanyRegisteredError>(() => result.Unwrap());
            Assert.Equal($"The supplier company with rif '{command.Rif}' is already registered.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_Company_When_Policy_Has_Invalid_Expiration_Date()
        {
            // Arrange
            var command = new RegisterSupplierCompanyCommand(
                [
                    new Application.Department("Contabilidad")
                ],
                [
                    new Application.Policy("Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(2), DateOnly.FromDateTime(DateTime.Now).AddDays(1))
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "c3889b66-4281-44fd-b731-57d7906eb566"
                ],
                "Gruas Caracas",
                "04145241547",
                "External",
                "J-87654321-9",
                "Distrito Capital",
                "Caracas",
                "Calle Paris"
            );

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindByRif(command.Rif))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Empty());

            // Act
            var result = await _registerSupplierCompanyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<InvalidPolicyExpirationDateError>(() => result.Unwrap());
            Assert.Equal("Policy expiration date less than the date of issuance.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_Company_When_TowDrivers_Are_Duplicated()
        {
            // Arrange
            var command = new RegisterSupplierCompanyCommand(
                [
                    new Application.Department("Contabilidad")
                ],
                [
                    new Application.Policy("Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddDays(2))
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd"
                ],
                "Gruas Caracas",
                "04145241547",
                "External",
                "J-87654321-9",
                "Distrito Capital",
                "Caracas",
                "Calle Paris"
            );

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindByRif(command.Rif))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Empty());

            // Act
            var result = await _registerSupplierCompanyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<DuplicateTowDriverError>(() => result.Unwrap());
            Assert.Equal("Some tow driver have registered twice.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        
        [Fact]
        public async Task Should_Register_Company()
        {
            // Arrange
            var command = new RegisterSupplierCompanyCommand(
                [
                    new Application.Department("Contabilidad")
                ],
                [
                    new Application.Policy("Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddDays(2))
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "3586e2f5-a5cf-435d-817a-3a2437185366"
                ],
                "Gruas Caracas",
                "04145241547",
                "External",
                "J-87654321-9",
                "Distrito Capital",
                "Caracas",
                "Calle Paris"
            );

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindByRif(command.Rif))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Empty());

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("da821989-6297-4c4a-8277-32deda0c52c7");

            // Act
            var result = await _registerSupplierCompanyCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("da821989-6297-4c4a-8277-32deda0c52c7", result.Unwrap().Id);

            /*_supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.Is<Domain.SupplierCompany>(supplierCompany =>
                    supplierCompany.GetSupplierCompanyId().GetValue() == "da821989-6297-4c4a-8277-32deda0c52c7" &&
                    supplierCompany.GetDepartments().Count() == command.Departments.Count &&
                    supplierCompany.GetDepartments().First().GetName().GetValue() == command.Departments[0].Name &&
                    supplierCompany.GetDepartments().First().GetEmployees().Count() == command.Departments.Count &&
                    supplierCompany.GetDepartments().First().GetEmployees().First().GetValue() == command.Departments[0].Employees[0] &&
                    supplierCompany.GetDepartments().First().GetEmployees().Last().GetValue() == command.Departments[0].Employees[1] &&
                    supplierCompany.GetPolicies().Count() == command.Policies.Count &&
                    supplierCompany.GetPolicies().First().GetTitle().GetValue() == command.Policies[0].Title &&
                    supplierCompany.GetPolicies().First().GetCoverageAmount().GetValue() == command.Policies[0].CoverageAmount &&
                    supplierCompany.GetPolicies().First().GetPrice().GetValue() == command.Policies[0].Price &&
                    supplierCompany.GetPolicies().First().GetType().GetValue() == command.Policies[0].Type &&
                    supplierCompany.GetPolicies().First().GetIssuanceDate().GetValue() == command.Policies[0].IssuanceDate &&
                    supplierCompany.GetPolicies().First().GetExpirationDate().GetValue() == command.Policies[0].ExpirationDate &&
                    supplierCompany.GetTowDrivers().Count() == command.TowDrivers.Count &&
                    supplierCompany.GetTowDrivers().First().GetValue() == command.TowDrivers[0] &&
                    supplierCompany.GetName().GetValue() == command.Name &&
                    supplierCompany.GetPhoneNumber().GetValue() == command.PhoneNumber &&
                    supplierCompany.GetType().GetValue() == command.Type &&
                    supplierCompany.GetRif().GetValue() == command.Rif &&
                    supplierCompany.GetAddress().GetState() == command.State &&
                    supplierCompany.GetAddress().GetCity() == command.City &&
                    supplierCompany.GetAddress().GetStreet() == command.Street
                )
            ), Times.Once);*/

            _eventStoreMock.Verify(store => store.AppendEvents(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 &&
                    events[0] is SupplierCompanyCreatedEvent
                )
            ), Times.Once);

            _messageBrokerServiceMock.Verify(service => service.Publish(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 &&
                    events[0] is SupplierCompanyCreatedEvent
                )
            ), Times.Once);
        } 
    }
}