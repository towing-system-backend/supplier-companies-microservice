namespace SupplierCompany.Application
{
    public record DepartmentUpdate(string Id, string Name, List<string> Employees);
    public record PolicyUpdate(string Id, string Title, int CoverageAmount, decimal Price, string Type, DateTime IssuanceDate, DateTime ExpirationDate);
    public record UpdateSupplierCompanyCommand(
        string Id,
        List<Department>? Departments,
        List<Policy>? Policies,
        List<string>? TowDrivers,
        string? Name,
        string? PhoneNumber,
        string? Type,
        string? Rif,
        string? State,
        string? City,
        string? Street
    );
}
