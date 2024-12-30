using Application.Core;
using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public record CreateDepartment(
        [Required]
        [StringLength(20, MinimumLength = 5)]
        string Name,
        [Required]
        [ValidateEachGuid(ErrorMessage = "Each tow driver ID must be a valid 'Guid'.")]
        List<string> Employee
    );

    public record CreatePolicy(
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
    
    public record CreateSupplierCompanyDto(
        [Required]
        List<CreateDepartment> Departments,
        [Required]
        List<CreatePolicy> Policies,
        [Required]
        [ValidateEachGuid(ErrorMessage = "Each tow driver ID must be a valid 'Guid'.")]
        List<string> TowDrivers,
        [Required]
        [StringLength(20, MinimumLength = 5)]
        string Name,
        [Required]
        [RegularExpression(@"^(0?4(14|24|16|26)\d{7})$", ErrorMessage = "Invalid phone number format.")]
        string PhoneNumber,
        [Required]
        [RegularExpression(@"^(Internal|External)$", ErrorMessage = "Type must be 'Internal' or 'External'.")]
        string Type,
        [Required]
        [RegularExpression(@"^J-\d{8}-\d$", ErrorMessage = "Invalid RIF format.")]
        string Rif,
        [Required]
        [StringLength(20, MinimumLength = 3)]
        string State,
        [Required]
        [StringLength(20, MinimumLength = 3)]
        string City,
        [Required]
        [StringLength(20, MinimumLength = 3)]
        string Street
    );
}