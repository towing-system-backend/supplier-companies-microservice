using Application.Core;
using MongoDB.Driver;
using SupplierCompany.Application;

namespace SupplierCompany.Infrastructure
{
    public class FindAllSupplierCompaniesQuery : IService<string, List<FindAllSupplierCompanyResponse>>
    {
        private readonly IMongoCollection<MongoSupplierCompany> _supplierCompanyCollection;

        public FindAllSupplierCompaniesQuery()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _supplierCompanyCollection = database.GetCollection<MongoSupplierCompany>("supplier-companies");
        }

        public async Task<Result<List<FindAllSupplierCompanyResponse>>> Execute(string param)
        {
            var res = await _supplierCompanyCollection.Find(_ => true).ToListAsync();

            if (res == null) return Result<List<FindAllSupplierCompanyResponse>>.MakeError(new SupplierCompanyNotFoundError());

            var response = res.Select(supplierCompany =>
            {
                var departments = supplierCompany.Departments.Select(d =>
                new DepartmentResponse(
                        d.DepartmentId,
                        d.Name,
                        d.Employees
                    )
                ).ToList();

                var policies = supplierCompany.Policies.Select(p =>
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

                return new FindAllSupplierCompanyResponse(
                    supplierCompany.SupplierCompanyId,
                    departments,
                    policies,
                    supplierCompany.TowDrivers,
                    supplierCompany.Name,
                    supplierCompany.PhoneNumber,
                    supplierCompany.Type,
                    supplierCompany.Rif,
                    supplierCompany.State,
                    supplierCompany.City,
                    supplierCompany.Street
                );
            }).ToList();

            return Result<List<FindAllSupplierCompanyResponse>>.MakeSuccess(response);
        }
    }
}