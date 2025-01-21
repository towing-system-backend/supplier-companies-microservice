namespace SupplierCompany.Application
{
    public record Department(string Name);
    public record Policy(
        string Title,
        int CoverageAmount,
        int CoverageDistance,
        decimal Price,
        string Type,
        DateOnly IssuanceDate,
        DateOnly ExpirationDate
    );

    public record RegisterSupplierCompanyCommand(
        List<Department> Departments,
        List<Policy> Policies,
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
