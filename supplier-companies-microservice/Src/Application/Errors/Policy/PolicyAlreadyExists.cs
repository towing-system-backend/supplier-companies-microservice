using Application.Core;

namespace SupplierCompany.Application
{
    public class PolicyAlreadyExistsError : ApplicationError
    {
        public PolicyAlreadyExistsError(string title) : base($"Policy with Title '{title}' already exists.") { }
    }
}
