using Application.Core;
using Moq;
using SupplierCompany.Application;
using SupplierCompany.Domain;
using Xunit;

namespace SupplierCompany.Test
{
    public class RegisterTowDriverCommandHandlerTests
    {
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<ISupplierCompanyRepository> _supplierCompanyRepositoryMock;
        private readonly RegisterTowDriverCommandHandler _registerTowDriverCommandHandler;

        public RegisterTowDriverCommandHandlerTests()
        {
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _eventStoreMock = new Mock<IEventStore>();
            _supplierCompanyRepositoryMock = new Mock<ISupplierCompanyRepository>();
            _registerTowDriverCommandHandler = new RegisterTowDriverCommandHandler(
                _messageBrokerServiceMock.Object,
                _eventStoreMock.Object,
                _supplierCompanyRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Register_TowDriver_When_SupplierCompany_Not_Found()
        {
            // Arrange
            var command = new RegisterTowDriverCommand("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b", "97d5a635-5cfc-4b2e-ab22-620417a0fe39");

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.SupplierCompanyId))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Empty());

            // Act
            var result = await _registerTowDriverCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<SupplierCompanyNotFoundError>(() => result.Unwrap());
            Assert.Equal("Supplier company not found.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_TowDriver_With_Invalid_Id()
        {
            // Arrange
            var command = new RegisterTowDriverCommand("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b", "invalid_uuid");

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
                        new PolicyPrice(5000),
                        new PolicyType("Especial"),
                        new PolicyIssuanceDate(DateTime.Now.AddDays(1)),
                        new PolicyExpirationDate(DateTime.Now.AddYears(1))
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

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.SupplierCompanyId))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _registerTowDriverCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<InvalidTowDriverError>(() => result.Unwrap());
            Assert.Equal("The tow driver id must be Guid.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_TowDriver_If_Already_Exists()
        {
            // Arrange
            var command = new RegisterTowDriverCommand("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b", "97d5a635-5cfc-4b2e-ab22-620417a0fe39");

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
                        new PolicyPrice(5000),
                        new PolicyType("Especial"),
                        new PolicyIssuanceDate(DateTime.Now.AddDays(1)),
                        new PolicyExpirationDate(DateTime.Now.AddYears(1))
                    )
                ],
                [
                    new TowDriverId("34624b38-b69f-4872-a939-25cc9723dd5f"),
                    new TowDriverId("97d5a635-5cfc-4b2e-ab22-620417a0fe39")
                ],
                new SupplierCompanyName("Gruas Caracas"),
                new SupplierCompanyPhoneNumber("04145241547"),
                new SupplierCompanyType("External"),
                new SupplierCompanyRif("J-87654321-9"),
                new SupplierCompanyAddress("Distrito Capital", "Caracas", "Calle Paris"),
                true
            );

            supplierCompany.RegisterTowDriver(new UserId("97d5a635-5cfc-4b2e-ab22-620417a0fe39"));

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.SupplierCompanyId))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _registerTowDriverCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<TowDriverAlreadyExistsError>(() => result.Unwrap());
            Assert.Equal($"Tow driver with id '{command.Id}' already exists.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Register_TowDriver_Successfully()
        {
            // Arrange
            var command = new RegisterTowDriverCommand("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b", "97d5a635-5cfc-4b2e-ab22-620417a0fe39");

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
                        new PolicyPrice(5000),
                        new PolicyType("Especial"),
                        new PolicyIssuanceDate(DateTime.Now.AddDays(1)),
                        new PolicyExpirationDate(DateTime.Now.AddYears(1))
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

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.SupplierCompanyId))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _registerTowDriverCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(command.Id, result.Unwrap().Id);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.Is<Domain.SupplierCompany>(sc =>
                sc.GetTowDrivers().Any(td => td.GetValue() == command.Id)
            )), Times.Once);

            _eventStoreMock.Verify(store => store.AppendEvents(It.Is<List<DomainEvent>>(events =>
                events.Count == 1 && events[0] is TowDriverRegisteredEvent
            )), Times.Once);

            _messageBrokerServiceMock.Verify(service => service.Publish(It.Is<List<DomainEvent>>(events =>
                events.Count == 1 && events[0] is TowDriverRegisteredEvent
            )), Times.Once);
        }
    }
}
