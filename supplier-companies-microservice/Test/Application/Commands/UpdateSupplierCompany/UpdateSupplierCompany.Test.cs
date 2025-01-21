using Application.Core;
using Moq;
using SupplierCompany.Application;
using SupplierCompany.Domain;
using Xunit;

namespace SupplierCompany.Test
{
    public class UpdateSupplierCompanyCommandHandlerTests
    {
        private readonly Mock<IdService<string>> _idServiceMock;
        private readonly Mock<IMessageBrokerService> _messageBrokerServiceMock;
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly Mock<ISupplierCompanyRepository> _supplierCompanyRepositoryMock;
        private readonly UpdateSupplierCompanyCommandHandler _updateSupplierCompanyCommandHandler;

        public UpdateSupplierCompanyCommandHandlerTests()
        {
            _idServiceMock = new Mock<IdService<string>>();
            _messageBrokerServiceMock = new Mock<IMessageBrokerService>();
            _eventStoreMock = new Mock<IEventStore>();
            _supplierCompanyRepositoryMock = new Mock<ISupplierCompanyRepository>();
            _updateSupplierCompanyCommandHandler = new UpdateSupplierCompanyCommandHandler(
                _idServiceMock.Object,
                _messageBrokerServiceMock.Object,
                _eventStoreMock.Object,
                _supplierCompanyRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Should_Not_Update_SupplierCompany_When_Company_Not_Found()
        {
            // Arrange
            var command = new UpdateSupplierCompanyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                [
                    new DepartmentUpdate("e1d33be1-1c00-4dd8-beb4-e8c0ab9aaa64", "Contabilidad", ["5631e557-08ac-475b-bd5f-a6cccc9da98d", "6426e7b8-3e17-4f0c-8273-01c7cc56ecdf"]),
                ],
                [
                    new PolicyUpdate("b791c9b1-a814-428c-847c-e25c92652dbf", "Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddDays(2)),
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "c3889b66-4281-44fd-b731-57d7906eb566",
                    "a95d8b6f-5331-4aee-990a-b943dbb81ed1",
                    "f4b5d5b9-6476-4799-a580-60eb5ab07891"
                ],
                "Gruas La Guaira",
                "04146522148",
                "Internal",
                "J-15748484-9",
                "Carabobo",
                "Valencia",
                "Avenida Bolivar"
            );

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Empty());

            // Act
            var result = await _updateSupplierCompanyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<SupplierCompanyNotFoundError>(() => result.Unwrap());
            Assert.Equal("Supplier company not found.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Update_SupplierCompany_With_Duplicate_Employees_In_Departments()
        {
            // Arrange
            var command = new UpdateSupplierCompanyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                [
                    new DepartmentUpdate("e1d33be1-1c00-4dd8-beb4-e8c0ab9aaa64", "Contabilidad", ["5631e557-08ac-475b-bd5f-a6cccc9da98d", "5631e557-08ac-475b-bd5f-a6cccc9da98d"]),
                ],
                [
                    new PolicyUpdate("b791c9b1-a814-428c-847c-e25c92652dbf", "Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddDays(2)),
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "c3889b66-4281-44fd-b731-57d7906eb566",
                    "a95d8b6f-5331-4aee-990a-b943dbb81ed1",
                    "f4b5d5b9-6476-4799-a580-60eb5ab07891"
                ],
                "Gruas La Guaira",
                "04146522148",
                "Internal",
                "J-15748484-9",
                "Carabobo",
                "Valencia",
                "Avenida Bolivar"
            );

            var supplierCompany = Domain.SupplierCompany.Create(
                new SupplierCompanyId("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b"),
                [new Domain.Department(
                    new DepartmentId("e1d33be1-1c00-4dd8-beb4-e8c0ab9aaa64"),
                    new DepartmentName("Finanzas"),
                    [new UserId("e525f54f-e032-4aad-aa01-995391abe173")])
                ],
                [new Domain.Policy(
                        new PolicyId("b791c9b1-a814-428c-847c-e25c92652dbf"),
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

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _updateSupplierCompanyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<DuplicateEmployeeError>(() => result.Unwrap());
            Assert.Equal("Some department has a duplicate employee.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Update_SupplierCompany_With_Invalid_Policy_Expiration_Date()
        {
            // Arrange
            var command = new UpdateSupplierCompanyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                [
                    new DepartmentUpdate("82219df5-e4a4-4ce6-95e8-42ac0af0a2a9", "Contabilidad", ["5631e557-08ac-475b-bd5f-a6cccc9da98d", "6426e7b8-3e17-4f0c-8273-01c7cc56ecdf"]),
                ],
                [
                    new PolicyUpdate("b68422ba-ebd0-45ea-9709-a53c64593d10", "Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(2), DateOnly.FromDateTime(DateTime.Now).AddDays(1)),
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "c3889b66-4281-44fd-b731-57d7906eb566",
                    "a95d8b6f-5331-4aee-990a-b943dbb81ed1",
                    "f4b5d5b9-6476-4799-a580-60eb5ab07891"
                ],
                "Gruas La Guaira",
                "04146522148",
                "Internal",
                "J-15748484-9",
                "Carabobo",
                "Valencia",
                "Avenida Bolivar"
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

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("2f196ebf-f89e-48b8-b75e-0e7febcfada1");

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("764700a1-1f59-4434-9af4-f70afabcff88");
            
            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _updateSupplierCompanyCommandHandler.Execute(command);

            // Assert
            Assert.True(result.IsError);
            var exception = Assert.Throws<InvalidPolicyExpirationDateError>(() => result.Unwrap());
            Assert.Equal("Policy expiration date less than the date of issuance.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        [Fact]
        public async Task Should_Not_Update_SupplierCompany_With_Duplicate_TowDrivers()
        {
            // Arrange
            var command = new UpdateSupplierCompanyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                [
                    new DepartmentUpdate("e1d33be1-1c00-4dd8-beb4-e8c0ab9aaa64", "Contabilidad", ["5631e557-08ac-475b-bd5f-a6cccc9da98d", "5631e557-08ac-475b-bd5f-a6cccc9da98d"]),
                ],
                [
                    new PolicyUpdate("b791c9b1-a814-428c-847c-e25c92652dbf", "Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddDays(2)),
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "c3889b66-4281-44fd-b731-57d7906eb566",
                    "a95d8b6f-5331-4aee-990a-b943dbb81ed1",
                    "f4b5d5b9-6476-4799-a580-60eb5ab07891"
                ],
                "Gruas La Guaira",
                "04146522148",
                "Internal",
                "J-15748484-9",
                "Carabobo",
                "Valencia",
                "Avenida Bolivar"
            );

            var supplierCompany = Domain.SupplierCompany.Create(
                new SupplierCompanyId("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b"),
                [new Domain.Department(
                    new DepartmentId("e1d33be1-1c00-4dd8-beb4-e8c0ab9aaa64"),
                    new DepartmentName("Finanzas"),
                    [new UserId("e525f54f-e032-4aad-aa01-995391abe173")])
                ],
                [new Domain.Policy(
                        new PolicyId("b791c9b1-a814-428c-847c-e25c92652dbf"),
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

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("2f196ebf-f89e-48b8-b75e-0e7febcfada1");

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("764700a1-1f59-4434-9af4-f70afabcff88");

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("34624b38-b69f-4872-a939-25cc9723dd5f");

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("74ac2a48-06f7-4cf6-8055-d17ad4902800");

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            // Act
            var result = await _updateSupplierCompanyCommandHandler.Execute(command);

            // Assert
            //Assert.True(result.IsError);
            var exception = Assert.Throws<DuplicateEmployeeError>(() => result.Unwrap());
            Assert.Equal("Some department has a duplicate employee.", exception.Message);

            _supplierCompanyRepositoryMock.Verify(repo => repo.Save(It.IsAny<Domain.SupplierCompany>()), Times.Never);
            _eventStoreMock.Verify(store => store.AppendEvents(It.IsAny<List<DomainEvent>>()), Times.Never);
            _messageBrokerServiceMock.Verify(service => service.Publish(It.IsAny<List<DomainEvent>>()), Times.Never);
        }

        
        [Fact]
        public async Task Should_Update_SupplierCompany_Successfully()
        {
            // Arrange
            var command = new UpdateSupplierCompanyCommand(
                "0ed13c45-93cf-4ef6-ba11-b386bb36bd1b",
                [
                    new DepartmentUpdate("e1d33be1-1c00-4dd8-beb4-e8c0ab9aaa64", "Contabilidad", ["5631e557-08ac-475b-bd5f-a6cccc9da98d"]),
                ],
                [
                    new PolicyUpdate("b791c9b1-a814-428c-847c-e25c92652dbf", "Paramedicos", 100000, 100, 5000, "Especial", DateOnly.FromDateTime(DateTime.Now).AddDays(1), DateOnly.FromDateTime(DateTime.Now).AddDays(2)),
                ],
                [
                    "dec6fb20-ee0f-4da2-a1c2-69e3b67810dd",
                    "c3889b66-4281-44fd-b731-57d7906eb566",
                    "a95d8b6f-5331-4aee-990a-b943dbb81ed1",
                    "f4b5d5b9-6476-4799-a580-60eb5ab07891"
                ],
                "Gruas La Guaira",
                "04146522148",
                "Internal",
                "J-15748484-9",
                "Carabobo",
                "Valencia",
                "Avenida Bolivar"
            );

            var supplierCompany = Domain.SupplierCompany.Create(
                new SupplierCompanyId("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b"),
                [new Domain.Department(
                    new DepartmentId("e1d33be1-1c00-4dd8-beb4-e8c0ab9aaa64"),
                    new DepartmentName("Finanzas"),
                    [new UserId("e525f54f-e032-4aad-aa01-995391abe173")])
                ],
                [new Domain.Policy(
                        new PolicyId("b791c9b1-a814-428c-847c-e25c92652dbf"),
                        new PolicyTitle("Accidente vial"),
                        new PolicyCoverageAmount(100000),
                        new PolicyCoverageDistance(100),
                        new PolicyPrice(5000),
                        new PolicyType("Especial"),
                        new PolicyIssuanceDate( DateOnly.FromDateTime(DateTime.Now).AddDays(1)),
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

            _supplierCompanyRepositoryMock.Setup(repo => repo.FindById(command.Id))
                .ReturnsAsync(Optional<Domain.SupplierCompany>.Of(supplierCompany));

            _idServiceMock.Setup(service => service.GenerateId())
                .Returns("0fedfe67-a21b-4da0-b0bf-48bac18ccf34");

            // Act
            var result = await _updateSupplierCompanyCommandHandler.Execute(command);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal("0ed13c45-93cf-4ef6-ba11-b386bb36bd1b", result.Unwrap().Id);
        }
    }
}
