using Application.Core;

namespace SupplierCompany.Domain
{
    public interface ISupplierCompanyRepository
    {
        Task<Optional<SupplierCompany>> FindById(string supplierCompanyId);
        Task<Optional<SupplierCompany>> FindByRif(string supplierCompanyRif);
        Task Save(SupplierCompany supplierCompany);
        Task Remove(string supplierCompanyId);
    }
}