namespace SupplierCompany.Application
{
    public record RegisterDepartmentCommand(
        string SupplierCompanyId,
        string Name,
        List<string> Employees
    );
}