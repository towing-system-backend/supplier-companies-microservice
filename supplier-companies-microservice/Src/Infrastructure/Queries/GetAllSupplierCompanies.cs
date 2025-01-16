using Application.Core;
using MongoDB.Driver;
using SupplierCompany.Infrastructure;

namespace SupplierCompany.Query
{
    public class GetAllSupplierCompanies
    {
        private readonly IMongoCollection<MongoSupplierCompany> _supplierCompanyCollection;

        public GetAllSupplierCompanies()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            var database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _supplierCompanyCollection = database.GetCollection<MongoSupplierCompany>("supplier-companies");
        }

        public async Task<Result<List<SupplierCompanyResponse>>> Execute()
        {
            var supplierCompanies = await _supplierCompanyCollection.Find(_ => true).ToListAsync();
            var response = supplierCompanies.Select(supplierCompany =>
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
                            p.Price,
                            p.Type,
                            p.IssuanceDate,
                            p.ExpirationDate
                        )
                    ).ToList();

                    return new SupplierCompanyResponse(
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

            return Result<List<SupplierCompanyResponse>>.MakeSuccess(response);
        }
    }
}