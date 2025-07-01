using System;
using System.ComponentModel.DataAnnotations;

namespace booking_platform.DTO;

public class ServiceProviderRequestDTO
{
    [Required(ErrorMessage = "Business ID is required.")]
    public Guid BusinessId { get; set; }
    public string UserId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    [StringLength(500, ErrorMessage = "Avatar image URL cannot exceed 500 characters.")]
    public string? AvatarImageUrl { get; set; }

    [Range(0, 6, ErrorMessage = "Available from weekday must be between 0 (Sunday) and 6 (Saturday).")]
    public int AvailableFromWeekday { get; set; } = 0;

    [Range(0, 6, ErrorMessage = "Available to weekday must be between 0 (Sunday) and 6 (Saturday).")]
    public int AvailableToWeekday { get; set; } = 6;

    [Required(ErrorMessage = "Available from time is required.")]
    public TimeSpan AvailableFromTime { get; set; }

    [Required(ErrorMessage = "Available to time is required.")]
    public TimeSpan AvailableToTime { get; set; }
}