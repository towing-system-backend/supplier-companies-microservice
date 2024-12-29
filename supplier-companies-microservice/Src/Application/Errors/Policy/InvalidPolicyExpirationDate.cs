using Application.Core;

namespace SupplierCompany.Application
{
    public class InvalidPolicyExpirationDateError : ApplicationError
    {
        public InvalidPolicyExpirationDateError() : base("Policy expiration date less than the date of issuance.") { }

    }
}
