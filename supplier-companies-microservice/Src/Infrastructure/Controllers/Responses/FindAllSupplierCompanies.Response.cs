namespace SupplierCompany.Infrastructure
{
    public record DepartmentResponse(

        string DepartmentId,
        string Name,
        List<string> Employees

    );

    public record PolicyResponse(
        string PolicyId,
        string Title,
        int CoverageAmount,
        int CoverageDistance,
        decimal Price,
        string Type,
        DateTime IssuanceDate,
        DateTime ExpirationDate
    );
    
    public record FindAllSupplierCompanyResponse(
        string SupplierCompanyId,
        List<DepartmentResponse> Departments,
        List<PolicyResponse> Policies,
        List<string> TowDrivers,
        string Name,
        string PhoneNumber,
        string Type,
        string Rif,
        string State,
        string City,
        string Street
    );
}