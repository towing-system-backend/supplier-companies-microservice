using Application.Core;
using MongoDB.Driver;
using SupplierCompany.Application;

namespace SupplierCompany.Infrastructure
{
    public class FindSupplierCompanyEmployeesQuery : IService<string, List<FindSupplierCompanyEmployeesResponse>>
    {
        private readonly IMongoCollection<MongoUser> _userCollection;

        public FindSupplierCompanyEmployeesQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _userCollection = database.GetCollection<MongoUser>("users");

            var indexKeysDefinition = Builders<MongoUser>.IndexKeys.Ascending(user => user.SupplierCompanyId).Ascending(user => user.Role);
            var indexModel = new CreateIndexModel<MongoUser>(indexKeysDefinition);
            _userCollection.Indexes.CreateOne(indexModel);
        }

        public async Task<Result<List<FindSupplierCompanyEmployeesResponse>>> Execute(string supplierCompanyId)
        {
            var filter = Builders<MongoUser>.Filter.And(
                Builders<MongoUser>.Filter.Eq(user => user.SupplierCompanyId, supplierCompanyId),
                Builders<MongoUser>.Filter.Eq(user => user.Role, "Employee"),
                Builders<MongoUser>.Filter.Eq(user => user.Status, "Active")
            );

            var res = await _userCollection.Find(filter).ToListAsync();

            if (res == null) return Result<List<FindSupplierCompanyEmployeesResponse>>.MakeError(new NoMatchesFoundError());

            var users = res.Select(user =>
                    new FindSupplierCompanyEmployeesResponse(
                    user.UserId,
                    user.SupplierCompanyId,
                    user.Name,
                    user.Image,
                    user.Email,
                    user.Role,
                    user.Status,
                    user.PhoneNumber,
                    user.IdentificationNumber
                )
            ).ToList();

            return Result<List<FindSupplierCompanyEmployeesResponse>>.MakeSuccess(users);
        }
    }
}