using Application.Core;

namespace SupplierCompany.Application
{
    public class NoMatchesFoundError : ApplicationError
    {
        public NoMatchesFoundError() : base("No matches found.") { }
    }
}