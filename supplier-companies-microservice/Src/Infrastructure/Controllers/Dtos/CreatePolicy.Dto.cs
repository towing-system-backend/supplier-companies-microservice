using System.ComponentModel.DataAnnotations;

namespace SupplierCompany.Infrastructure
{
    public record CreatePolicyDto(
        [Required]
        [RegularExpression(@"^([0-9A-Fa-f]{8}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{4}[-]?[0-9A-Fa-f]{12})$", ErrorMessage = "Id must be a 'Guid'.")]
        string SupplierCompanyId,
        [Required]
        string Title,
        [Required][Range(1, int.MaxValue)]
        int CoverageAmount,
        [Required][Range(1, int.MaxValue)]
        int CoverageDistance,
        [Required]
        decimal Price,
        [Required]
        string Type,
        [Required]
        DateOnly IssuanceDate,
        [Required]
        DateOnly ExpirationDate
    );
}