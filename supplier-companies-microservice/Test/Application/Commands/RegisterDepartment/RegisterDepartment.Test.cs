using Application.Core;
using Moq;
using SupplierCompany.Application;
using SupplierCompany.Domain;
using Xunit;

namespace SupplierCompany.Test
{
    public class RegisterDepartmentCommandHandlerTests
    {
        private readonly Mock<IdService<string>> _idServiceMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<ISupplierCompanyRepository> _supplierCompanyRepositoryMock;
        private readonly RegisterDepartmentCommandHandler _registerDepartmentCommandHandler;

        public RegisterDepartmentCommandHandlerTests()
        {
            _idServiceMock = new Mock<IdService<string>>();
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _eventStoreMock = new Mock<IEventStore>();
            _supplierCompanyRepositoryMock = new Mock<ISupplierCompanyRepository>();
            _registerDepartmentCommandHandler = new RegisterDepartmentCommandHandler(
                _idServiceMock.Object,
                _messageBrokerServiceMock.Object,
                _eventStoreMock.Object,
                _supplierCompanyRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Register_Department_When_SupplierCompany_Not_Found()
        {
            // Arrange
            var command = new RegisterDepartmentCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                "Finanzas",
                ["7909db07-1695-4959-9dc4-05a21e43b50d", "74956acc-6c84-4406-aa88-31596839ac8d"]
            );

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.SupplierCompanyId))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Empty());

            // Act
            var result = await _registerDepartmentCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<SupplierCompanyNotFoundError>(() => result.Unwrap());
            Assert.Equal("Supplier company not found.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_Department_When_Department_Already_Exists()
        {
            // Arrange
            var command = new RegisterDepartmentCommand(
                 "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                 "Tecnologia",
                 ["7909db07-1695-4959-9dc4-05a21e43b50d", "74956acc-6c84-4406-aa88-31596839ac8d"]
             );

            var supplierCompany = Domain.SupplierCompany.Create(
                new SupplierCompanyId("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b"),
                [new Domain.Department(
                    new DepartmentId("82219df5-e4a4-4ce6-95e8-42ac0af0a2a9"),
                    new DepartmentName("Tecnologia"),
                    [new UserId("e525f54f-e032-4aad-aa01-995391abe173")])
                ],
                [new Domain.Policy(
                        new PolicyId("b68422ba-ebd0-45ea-9709-a53c64593d10"),
                        new PolicyTitle("Policy Title"),
                        new PolicyCoverageAmount(100000),
                        new PolicyCoverageDistance(100),
                        new PolicyPrice(5000),
                        new PolicyType("Type"),
                        new PolicyIssuanceDate(DateTime.Now.AddDays(1)),
                        new PolicyExpirationDate(DateTime.Now.AddDays(2))
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

            _supplierCompanyRepositoryMock.Setup(repo =>
                repo.FindById(command.SupplierCompanyId)
            )
            .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _registerDepartmentCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<DepartmentAlreadyExistsError>(() => result.Unwrap());
            Assert.Equal($"Department with name '{command.Name}' already exists.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_Department_When_Some_EmployeeId_Is_Not_A_Valid_GUID()
        {
            // Arrange
            var command = new RegisterDepartmentCommand(
                 "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                 "Tecnologia",
                 ["7909db07-1695-4959-9dc4-05a21e43b50d", "Not_A_GUID"]
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
                        new PolicyTitle("Policy Title"),
                        new PolicyCoverageAmount(100000),
                        new PolicyCoverageDistance(100),
                        new PolicyPrice(5000),
                        new PolicyType("Type"),
                        new PolicyIssuanceDate(DateTime.Now.AddDays(1)),
                        new PolicyExpirationDate(DateTime.Now.AddDays(2))
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

            _supplierCompanyRepositoryMock.Setup(repo =>
                repo.FindById(command.SupplierCompanyId)
            )
            .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _registerDepartmentCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<InvalidEmployeeError>(() => result.Unwrap());
            Assert.Equal("The employee id must be Guid.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_Department_When_Some_EmployeeId_Is_Repeated()
        {
            // Arrange
            var command = new RegisterDepartmentCommand(
                 "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                 "Tecnologia",
                 ["7909db07-1695-4959-9dc4-05a21e43b50d", "7909db07-1695-4959-9dc4-05a21e43b50d"]
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
                        new PolicyTitle("Policy Title"),
                        new PolicyCoverageAmount(100000),
                        new PolicyCoverageDistance(100),
                        new PolicyPrice(5000),
                        new PolicyType("Type"),
                        new PolicyIssuanceDate(DateTime.Now.AddDays(1)),
                        new PolicyExpirationDate(DateTime.Now.AddDays(2))
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

            _supplierCompanyRepositoryMock.Setup(repo =>
                repo.FindById(command.SupplierCompanyId)
            )
            .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _registerDepartmentCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<DuplicateEmployeeError>(() => result.Unwrap());
            Assert.Equal("Some department has a duplicate employee.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Register_Department()
        {
            var command = new RegisterDepartmentCommand(
                 "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                 "Tecnologia",
                 ["7909db07-1695-4959-9dc4-05a21e43b50d", "74956acc-6c84-4406-aa88-31596839ac8d"]
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
                        new PolicyTitle("Policy Title"),
                        new PolicyCoverageAmount(100000),
                        new PolicyCoverageDistance(100),
                        new PolicyPrice(5000),
                        new PolicyType("Type"),
                        new PolicyIssuanceDate(DateTime.Now.AddDays(1)),
                        new PolicyExpirationDate(DateTime.Now.AddDays(2))
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

            _supplierCompanyRepositoryMock.Setup(repo =>
                repo.FindById(command.SupplierCompanyId)
            )
            .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            _idServiceMock.Setup(service => service.GenerateId()).Returns("d221653b-03de-489c-9209-bc5422a9dec8");

            // Act
            var result = await _registerDepartmentCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("d221653b-03de-489c-9209-bc5422a9dec8", result.Unwrap().Id);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.Is<Domain.SupplierCompany>(company =>
                    company.GetDepartments().Count == 2 &&
                    company.GetDepartments().Any(department => department.GetName().GetValue().Equals("Finanzas")) &&
                    company.GetDepartments().Any(department => department.GetName().GetValue().Equals("Tecnologia"))
                )
            ), Times.Once);
           

            _eventStoreMock.Verify(store => store.AppendEvents(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 &&
                    events[0] is DepartmentRegisteredEvent
                )
            ), Times.Once);

            _messageBrokerServiceMock.Verify(service => service.Publish(It.Is<List<DomainEvent>>(events =>
                    events.Count == 1 &&
                    events[0] is DepartmentRegisteredEvent
                )
            ), Times.Once);
        }
    }
}