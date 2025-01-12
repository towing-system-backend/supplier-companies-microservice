using Application.Core;
using Moq;
using SupplierCompany.Application;
using SupplierCompany.Domain;
using Xunit;

namespace SupplierCompany.Test
{
    public class RegisterPolicyCommandHandlerTests
    {
        private readonly Mock<IdService<string>> _idServiceMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<ISupplierCompanyRepository> _supplierCompanyRepositoryMock;
        private readonly RegisterPolicyCommandHandler _registerPolicyCommandHandler;

        public RegisterPolicyCommandHandlerTests()
        {
            _idServiceMock = new Mock<IdService<string>>();
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _eventStoreMock = new Mock<IEventStore>();
            _supplierCompanyRepositoryMock = new Mock<ISupplierCompanyRepository>();
            _registerPolicyCommandHandler = new RegisterPolicyCommandHandler(
                _idServiceMock.Object,
                _messageBrokerServiceMock.Object,
                _eventStoreMock.Object,
                _supplierCompanyRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Register_Policy_When_SupplierCompany_Not_Found()
        {
            // Arrange
            var command = new RegisterPolicyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                "Accidente vial",
                100000,
                5000,
                "Especial",
                DateTime.Now,
                DateTime.Now.AddYears(1)
            );

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.SupplierCompanyId))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Empty());

            // Act
            var result = await _registerPolicyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<SupplierCompanyNotFoundError>(() => result.Unwrap());
            Assert.Equal("Supplier company not found.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_Policy_When_Policy_Already_Exists()
        {
            // Arrange
            var command = new RegisterPolicyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                "Accidente vial",
                100000,
                5000,
                "Especial",
                DateTime.Now,
                DateTime.Now.AddYears(1)
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
            var result = await _registerPolicyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<PolicyAlreadyExistsError>(() => result.Unwrap());
            Assert.Equal($"Policy with Title '{command.Title}' already exists.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Register_Policy_With_Invalid_Expiration_Date()
        {
            // Arrange
            var command = new RegisterPolicyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                "Paramedicos",
                100000,
                5000,
                "Especial",
                DateTime.Now,
                DateTime.Now.AddDays(-1)
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
            var result = await _registerPolicyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<InvalidPolicyExpirationDateError>(() => result.Unwrap());
            Assert.Equal("Policy expiration date less than the date of issuance.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Register_Policy()
        {
            // Arrange
            var command = new RegisterPolicyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                "Paramedicos",
                100000,
                5000,
                "Especial",
                DateTime.Now.AddDays(1),
                DateTime.Now.AddYears(1)
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

            _idServiceMock.Setup(service => service.GenerateId()).Returns("e739fe78-53a8-4370-9a8f-b3e75f04cb0c");

            // Act
            var result = await _registerPolicyCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("e739fe78-53a8-4370-9a8f-b3e75f04cb0c", result.Unwrap().Id);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.Is<Domain.SupplierCompany>(company =>
                company.GetPolicies().Any(policy =>
                    policy.GetId().GetValue() == "e739fe78-53a8-4370-9a8f-b3e75f04cb0c" &&
                    policy.GetTitle().GetValue() == command.Title &&
                    policy.GetCoverageAmount().GetValue() == command.CoverageAmount &&
                    policy.GetPrice().GetValue() == command.Price &&
                    policy.GetType().GetValue() == command.Type &&
                    policy.GetIssuanceDate().GetValue() == command.IssuanceDate &&
                    policy.GetExpirationDate().GetValue() == command.ExpirationDate
                )
            )), Times.Once);

            _eventStoreMock.Verify(store => store.AppendEvents(It.Is<List<DomainEvent>>(events =>
                events.Count == 1 &&
                events[0] is PolicyRegisteredEvent
            )), Times.Once);

            _messageBrokerServiceMock.Verify(service => service.Publish(It.Is<List<DomainEvent>>(events =>
                events.Count == 1 &&
                events[0] is PolicyRegisteredEvent
            )), Times.Once);
        } 
    }
}
