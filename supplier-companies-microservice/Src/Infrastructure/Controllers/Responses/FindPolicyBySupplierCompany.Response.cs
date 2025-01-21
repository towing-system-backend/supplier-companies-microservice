namespace SupplierCompany.Infrastructure
{
    public record FindPolicyBySupplierCompanyResponse(
        string PolicyId,
        string Title,
        int CoverageAmount,
        int CoverageDistance,
        decimal Price,
        string Type,
        DateOnly IssuanceDate,
        DateOnly ExpirationDate
    );
}