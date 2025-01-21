namespace SupplierCompany.Application
{
    public record RegisterPolicyCommand(
        string SupplierCompanyId,
        string Title,
        int CoverageAmount,
        int CoverageDistance,
        decimal Price,
        string Type,
        DateOnly IssuanceDate,
        DateOnly ExpirationDate
    );
}
