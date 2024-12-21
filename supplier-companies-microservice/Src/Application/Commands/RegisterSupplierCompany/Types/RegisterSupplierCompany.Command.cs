﻿namespace SupplierCompany.Application
{
    public record Department(string Name, List<string> Employees);
    public record Policy(string Title, int CoverageAmount, decimal Price, string Type, DateTime IssuanceDate, DateTime ExpirationDate);
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
