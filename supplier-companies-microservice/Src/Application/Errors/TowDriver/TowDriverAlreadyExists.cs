using Application.Core;

namespace SupplierCompany.Application
{
    public class TowDriverAlreadyExistsError : ApplicationError
    {
        public TowDriverAlreadyExistsError(string id) : base($"Tow driver with id {id} already exists.") { }
    }
}
