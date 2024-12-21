using Application.Core;

namespace SupplierCompany.Domain
{
    public interface ISupplierCompanyRepository
    {
        Task<Optional<SupplierCompany>> FindById(string supplierCompanyId);
        Task Save(SupplierCompany supplierCompany);
        Task Remove(string supplierCompanyId);
    }
}