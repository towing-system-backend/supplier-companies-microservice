using Application.Core;
using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public record UpdateDepartment(
        [Required][RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string DepartmentId,
        [Required][StringLength(20, MinimumLength = 5)]
        string Name,
        [ValidateEachGuid(ErrorMessage = "Each employee ID must be a valid 'Guid'.")]
        List<string>? Employees
    );

    public record UpdatePolicy(
        [Required][RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string PolicyId,
        [Required][StringLength(20, MinimumLength = 5)]
        string Title,
        [Required][Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero.")]
        int CoverageAmount,
        [Required][Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero.")]
        int CoverageDistance,
        [Required][Range(1, double.MaxValue, ErrorMessage = "Value must be greater than zero.")]
        decimal Price,
        [Required][StringLength(20, MinimumLength = 5)]
        string Type,
        [Required]
        DateTime IssuanceDate,
        [Required]
        DateTime ExpirationDate
    );

    public record UpdateSupplierCompanyDto(
        List<UpdateDepartment>? Departments,
        List<UpdatePolicy>? Policies,
        [ValidateEachGuid(ErrorMessage = "Each tow driver ID must be a valid 'Guid'.")]
        List<string>? TowDrivers,
        [Required]
        [RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string SupplierCompanyId,
        [StringLength(20, MinimumLength = 5)]
        string? Name,
        [RegularExpression(@"^(0?4(14|24|16|26)\d{7})$", ErrorMessage = "Invalid phone number format.")]
        string? PhoneNumber,
        [RegularExpression(@"^(Internal|External)$", ErrorMessage = "Type must be 'Internal' or 'External'.")]
        string? Type,
        [RegularExpression(@"^J-\d{8}-\d$", ErrorMessage = "Invalid RIF format.")]
        string? Rif,
        [StringLength(20, MinimumLength = 3)]
        string? State,
        [StringLength(20, MinimumLength = 3)]
        string? City,
        [StringLength(20, MinimumLength = 3)]
        string? Street
    );
}