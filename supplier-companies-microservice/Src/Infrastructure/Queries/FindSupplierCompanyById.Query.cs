using Application.Core;
using MongoDB.Driver;
using SupplierCompany.Application;

namespace SupplierCompany.Infrastructure
{
    public class FindSupplierCompanyByIdQuery : IService<string, FindSupplierCompanyByIdResponse>
    {
        private readonly IMongoCollection<MongoSupplierCompany> _supplierCompanyCollection;

        public FindSupplierCompanyByIdQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI_READ_MODELS"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME_READ_MODELS"));
            _supplierCompanyCollection = database.GetCollection<MongoSupplierCompany>("supplier-companies");
        }

        public async Task<Result<FindSupplierCompanyByIdResponse>> Execute(string supplierCompanyId)
        {
            var filter = Builders<MongoSupplierCompany>.Filter.Eq(supplierCompany => supplierCompany.SupplierCompanyId, supplierCompanyId);
            var res = await _supplierCompanyCollection.Find(filter).FirstOrDefaultAsync();

            if (res == null) return Result<FindSupplierCompanyByIdResponse>.MakeError(new SupplierCompanyNotFoundError());

            var departments = res.Departments.Select(d =>
                new DepartmentResponse(
                    d.DepartmentId,
                    d.Name,
                    d.Employees
                )
            ).ToList();

            var policies = res.Policies.Select(p =>
                new PolicyResponse(
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

            var towDrivers = res.TowDrivers;

            var supplierCompany = new FindSupplierCompanyByIdResponse(
                res.SupplierCompanyId,
                departments,
                policies,
                towDrivers,
                res.Name,
                res.PhoneNumber,
                res.Type,
                res.Rif,
                res.State,
                res.City,
                res.Street
            );

            return Result<FindSupplierCompanyByIdResponse>.MakeSuccess(supplierCompany);
        }
    }
}