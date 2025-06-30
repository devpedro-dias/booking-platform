using System.ComponentModel.DataAnnotations;

namespace booking_platform.DTO;

public class ServiceRequestDTO
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Price must be a non-negative value.")]
    public int PriceInCents { get; set; }

    [Required(ErrorMessage = "Duration is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
    public int DurationInMinutes { get; set; }

    [Required(ErrorMessage = "Business ID is required.")]
    public Guid BusinessId { get; set; }
}