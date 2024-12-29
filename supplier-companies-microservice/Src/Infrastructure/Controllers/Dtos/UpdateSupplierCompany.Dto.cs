using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public record UpdateDepartment(
        [Required]
        [StringLength(20, MinimumLength = 5)]
        string Name,
        [Required]
        List<string> Employee
    );

    public record UpdatePolicy(
        [Required]
        [StringLength(20, MinimumLength = 5)]
        string Title,
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero.")]
        int CoverageAmount,
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Value must be greater than zero.")]
        decimal Price,
        [Required]
        [StringLength(20, MinimumLength = 5)]
        string Type,
        [Required]
        DateTime IssuanceDate,
        [Required]
        DateTime ExpirationDate
    );

    public record UpdateSupplierCompanyDto(
        List<UpdateDepartment>? Departments,
        List<UpdatePolicy>? Policies,
        List<string>? TowDrivers,
        [Required]
        string Id,
        [StringLength(20, MinimumLength = 5)]
        string? Name,
        [RegularExpression(@"^(0?4(14|24|16|26)\d{7})$", ErrorMessage = "Invalid phone number format")]
        string? PhoneNumber,
        [RegularExpression(@"^(Internal|External)$", ErrorMessage = "Type must be 'Internal', or 'External'")]
        string? Type,
        [RegularExpression(@"^J-\d{8}-\d$", ErrorMessage = "Invalid RIF format")]
        string? Rif,
        [StringLength(20, MinimumLength = 3)]
        string? State,
        [StringLength(20, MinimumLength = 3)]
        string? City,
        [StringLength(20, MinimumLength = 3)]
        string? Street
    );
}