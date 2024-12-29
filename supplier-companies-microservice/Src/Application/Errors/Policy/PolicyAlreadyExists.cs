using Application.Core;

namespace SupplierCompany.Application
{
    public class PolicyAlreadyExistsError : ApplicationError
    {
        public PolicyAlreadyExistsError(string policyId) : base($"Policy with rif {policyId} already exists.") { }
    }
}
