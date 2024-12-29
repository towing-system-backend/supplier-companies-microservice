using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public record CreateDepartmentDto(
        [Required]
        string SupplierCompanyId,
        [Required]
        string Name,
        [Required]
        List<string> Employees
    );
}