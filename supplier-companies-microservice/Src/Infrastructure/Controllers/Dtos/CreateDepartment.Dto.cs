using Application.Core;
using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public record CreateDepartmentDto(
        [Required]
        [RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string SupplierCompanyId,
        [Required]
        string Name,
        [Required]
        [ValidateEachGuid(ErrorMessage = "Each employee ID must be a valid 'Guid'.")]
        List<string> Employees
    );
}