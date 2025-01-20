using Application.Core;
using MongoDB.Driver;
using SupplierCompany.Application;

namespace SupplierCompany.Infrastructure
{
    public class FindSupplierCompanyTowDriversQuery : IService<string, List<FindSupplierCompanyTowDriversResponse>>
    {
        private readonly IMongoCollection<MongoTowDriver> _towDriverCollection;

        public FindSupplierCompanyTowDriversQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _towDriverCollection = database.GetCollection<MongoTowDriver>("tow-drivers");

            var indexKeysDefinition = Builders<MongoTowDriver>.IndexKeys
                .Ascending(towDriver => towDriver.SupplierCompanyId)
                .Ascending(towDriver => towDriver.Status);

            var indexModel = new CreateIndexModel<MongoTowDriver>(indexKeysDefinition);
            _towDriverCollection.Indexes.CreateOne(indexModel);
        }

        public async Task<Result<List<FindSupplierCompanyTowDriversResponse>>> Execute(string supplierCompanyId)
        {
            var filter = Builders<MongoTowDriver>.Filter.And(
                Builders<MongoTowDriver>.Filter.Eq(towDriver => towDriver.SupplierCompanyId, supplierCompanyId),
                Builders<MongoTowDriver>.Filter.Eq(towDriver => towDriver.Status, "Active")
            );

            var res = await _towDriverCollection.Find(filter).ToListAsync();

            if (res == null) return Result<List<FindSupplierCompanyTowDriversResponse>>.MakeError(new NoMatchesFoundError());

            var towDrivers = res.Select(towDriver =>
                new FindSupplierCompanyTowDriversResponse(
                    towDriver.TowDriverId,
                    towDriver.Name,
                    towDriver.Email,
                    towDriver.DrivingLicenseOwnerName,
                    towDriver.DrivingLicenseIssueDate,
                    towDriver.DrivingLicenseExpirationDate,
                    towDriver.MedicalCertificateOwnerName,
                    towDriver.MedicalCertificateOwnerAge,
                    towDriver.MedicalCertificateIssueDate,
                    towDriver.MedicalCertificateExpirationDate,
                    towDriver.IdentificationNumber,
                    towDriver.Location!,
                    towDriver.Status!
                )
            ).ToList();

            return Result<List<FindSupplierCompanyTowDriversResponse>>.MakeSuccess(towDrivers);
        }
    }
}