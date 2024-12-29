using Application.Core;

namespace SupplierCompany.Application
{
    public class SupplierCompanyNotFoundError : ApplicationError
    {
        public SupplierCompanyNotFoundError() : base("Supplier company not found.") { }
    }
}
