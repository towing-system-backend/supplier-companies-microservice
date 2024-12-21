using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public enum PolicyTypeDto
    {
        Internal,
        External
    }

    public record DepartmentDto(
        [Required]
        [StringLength(20, MinimumLength = 5)]
        string Name,
        [Required]
        List<string> Employee
    );

    public record PolicyDto(
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
        [EnumDataType(typeof(PolicyTypeDto))]
        string Type,
        [Required]
        DateTime IssuanceDate,
        [Required]
        DateTime ExpirationDate
    );
    public record CreateSupplierCompanyDto(
        [Required]
        List<DepartmentDto> Departments,
        [Required]
        List<PolicyDto> Policies,
        [Required]
        List<string> TowDrivers,
        [Required]
        [StringLength(20, MinimumLength = 5)]
        string Name,
        [Required]
        [RegularExpression(@"^(0?4(14|24|16|26)\d{7})$", ErrorMessage = "Invalid phone number format")]
        string PhoneNumber,
        [Required]
        string Type,
        [Required]
        [RegularExpression(@"^J-\d{8}-\d$", ErrorMessage = "Invalid RIF format")]
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