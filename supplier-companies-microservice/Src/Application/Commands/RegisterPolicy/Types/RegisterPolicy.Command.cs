namespace SupplierCompany.Application
{
    public record RegisterPolicyCommand(
        string SupplierCompanyId,
        string Title,
        int CoverageAmount,
        decimal Price,
        string Type,
        DateTime IssuanceDate,
        DateTime ExpirationDate
    );
}
