using Application.Core;
using MongoDB.Driver;
using SupplierCompany.Application;

namespace SupplierCompany.Infrastructure
{
    public class FindPolicyBySupplierCompanyQuery : IService<string, List<FindPolicyBySupplierCompanyResponse>>
    {
        private readonly IMongoCollection<MongoSupplierCompany> _supplierCompanyCollection;

        public FindPolicyBySupplierCompanyQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _supplierCompanyCollection = database.GetCollection<MongoSupplierCompany>("supplier-companies");
        }

        public async Task<Result<List<FindPolicyBySupplierCompanyResponse>>> Execute(string supplierCompanyId)
        {
            
            var projection = Builders<MongoSupplierCompany>.Projection
                .Include(x => x.Policies);

            var supplierCompany = await _supplierCompanyCollection
                .Find(sc => sc.SupplierCompanyId == supplierCompanyId)
                .Project<MongoSupplierCompany>(projection)
                .FirstOrDefaultAsync();

            if (supplierCompany == null)
                return Result<List<FindPolicyBySupplierCompanyResponse>>.MakeError(new SupplierCompanyNotFoundError());
    
            var policies = supplierCompany.Policies.Select(p =>
                new FindPolicyBySupplierCompanyResponse(
                    p.PolicyId,
                    p.Title,
                    p.CoverageAmount,
                    p.CoverageDistance,
                    p.Price,
                    p.Type,
                    p.IssuanceDate,
                    p.ExpirationDate
                )
            ).ToList();

            return Result<List<FindPolicyBySupplierCompanyResponse>>.MakeSuccess(policies);
        }
    }
}