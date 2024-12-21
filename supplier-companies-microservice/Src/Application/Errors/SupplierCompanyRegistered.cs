using Application.Core;

namespace SupplierCompany.Application
{
    public class SupplierCompanyRegisteredError : ApplicationError
    {
        public SupplierCompanyRegisteredError(string rif) : base($"The supplier company with rif {rif} is already registered.") { }
    }
}
