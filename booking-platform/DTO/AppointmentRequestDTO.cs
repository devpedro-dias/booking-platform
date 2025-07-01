using System;
using System.ComponentModel.DataAnnotations;

namespace booking_platform.DTO;

public class AppointmentRequestDTO
{
    [Required(ErrorMessage = "Start Date and Time is required.")]
    public DateTime StartDateTime { get; set; }

    [Required(ErrorMessage = "End Date and Time is required.")]
    public DateTime EndDateTime { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
    public string Status { get; set; }

    [Required(ErrorMessage = "Business ID is required.")]
    public Guid BusinessId { get; set; }

    [Required(ErrorMessage = "Customer ID is required.")]
    public string CustomerId { get; set; }

    [Required(ErrorMessage = "Service Provider ID is required.")]
    public Guid ServiceProviderId { get; set; }

    [Required(ErrorMessage = "Service ID is required.")]
    public Guid ServiceId { get; set; }
}