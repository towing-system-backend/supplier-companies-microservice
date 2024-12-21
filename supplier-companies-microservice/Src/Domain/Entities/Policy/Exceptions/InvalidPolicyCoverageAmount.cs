using Application.Core;

namespace SupplierCompany.Domain
{
    public class InvalidPolicyCoverageAmountException : DomainException
    {
        public InvalidPolicyCoverageAmountException() : base("Invalid policy coverage amount.") { }
    }
}