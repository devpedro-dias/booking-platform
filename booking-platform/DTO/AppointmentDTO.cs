namespace booking_platform.DTO;

public class AppointmentDTO
{
    public string Id { get; set; }
    public string BusinessId { get; set; }
    public string CustomerId { get; set; }
    public string ServiceId { get; set; }
    public string ServiceProviderId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int TotalPriceInCents { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
