using MongoDB.Driver;
using SupplierCompany.Domain;
using IOptional = Application.Core.Optional<SupplierCompany.Domain.SupplierCompany>;

namespace SupplierCompany.Infrastructure
{
    public class MongoSupplierCompanyRepository : ISupplierCompanyRepository
    {
        private readonly IMongoCollection<MongoSupplierCompany> _supplierCompanyCollection;
        public MongoSupplierCompanyRepository()
        {
            MongoClient client = new MongoClient(Environment.GetEnvironmentVariable("CONNECTION_URI"));
            IMongoDatabase database = client.GetDatabase(Environment.GetEnvironmentVariable("DATABASE_NAME"));
            _supplierCompanyCollection = database.GetCollection<MongoSupplierCompany>("supplier-companies");
        }

        public async Task<IOptional> FindById(string id)
        {
            var filter = Builders<MongoSupplierCompany>.Filter.Eq(supplierCompany => supplierCompany.SupplierCompanyId, id);
            var res = await _supplierCompanyCollection.Find(filter).FirstOrDefaultAsync();

            if (res == null) return IOptional.Empty();

            var departments = res.Departments.Select(d =>
                new Department(
                    new DepartmentId(d.DepartmentId),
                    new DepartmentName(d.Name),
                    d.Employees.Select(e => new UserId(e)).ToList()
                )
            ).ToList();

            var policies = res.Policies.Select(p =>
                new Policy(
                    new PolicyId(p.PolicyId),
                    new PolicyTitle(p.Title),
                    new PolicyCoverageAmount(p.CoverageAmount),
                    new PolicyPrice(p.Price),
                    new PolicyType(p.Type),
                    new PolicyIssuanceDate(p.IssuanceDate),
                    new PolicyExpirationDate(p.ExpirationDate)
                )
            ).ToList();

            var towDrivers = res.TowDrivers.Select(t => new TowDriverId(t)).ToList();

            return IOptional.Of(
                Domain.SupplierCompany.Create(
                    new SupplierCompanyId(res.SupplierCompanyId),
                    departments,
                    policies,
                    towDrivers,
                    new SupplierCompanyName(res.Name),
                    new SupplierCompanyPhoneNumber(res.PhoneNumber),
                    new SupplierCompanyType(res.Type),
                    new SupplierCompanyRif(res.Rif),
                    new SupplierCompanyAddress(res.State, res.City, res.Street),
                    true
                )
            );
        }

        public async Task Save(Domain.SupplierCompany supplierCompany)
        {
            var filter = Builders<MongoSupplierCompany>.Filter.Eq(supplierCompany => supplierCompany.SupplierCompanyId, supplierCompany.GetSupplierCompanyId().GetValue());
            var update = Builders<MongoSupplierCompany>.Update
                .Set(supplierCompany => supplierCompany.Departments, supplierCompany.GetDepartments().Select(d =>
                    new MongoDepartment(d.GetId().GetValue(), d.GetName().GetValue(), d.GetEmployees().Select(e => e.GetValue()).ToList())).ToList())
                .Set(supplierCompany => supplierCompany.Policies, supplierCompany.GetPolicies().Select(p =>
                    new MongoPolicy(
                        p.GetId().GetValue(),
                        p.GetTitle().GetValue(),
                        p.GetCoverageAmount().GetValue(),
                        p.GetPrice().GetValue(),
                        p.GetType().GetValue(),
                        p.GetIssuanceDate().GetValue(),
                        p.GetExpirationDate().GetValue()
                    )).ToList())
                .Set(supplierCompany => supplierCompany.TowDrivers, supplierCompany.GetTowDrivers().Select(t => t.GetValue()).ToList())
                .Set(supplierCompany => supplierCompany.Name, supplierCompany.GetName().GetValue())
                .Set(supplierCompany => supplierCompany.PhoneNumber, supplierCompany.GetPhoneNumber().GetValue())
                .Set(supplierCompany => supplierCompany.Type, supplierCompany.GetType().GetValue())
                .Set(supplierCompany => supplierCompany.Rif, supplierCompany.GetRif().GetValue())
                .Set(supplierCompany => supplierCompany.State, supplierCompany.GetAddress().GetState())
                .Set(supplierCompany => supplierCompany.City, supplierCompany.GetAddress().GetCity())
                .Set(supplierCompany => supplierCompany.Street, supplierCompany.GetAddress().GetStreet());

            await _supplierCompanyCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public async Task Remove(string id)
        {
            var filter = Builders<MongoSupplierCompany>.Filter.Eq(supplierCompany => supplierCompany.SupplierCompanyId, id);

            await _supplierCompanyCollection.DeleteOneAsync(filter);
        }
    }
}