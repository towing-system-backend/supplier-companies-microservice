namespace SupplierCompany.Infrastructure
{
    public record FindSupplierCompanyByIdResponse(
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