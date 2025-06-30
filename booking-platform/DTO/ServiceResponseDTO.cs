namespace booking_platform.DTO;

public class ServiceResponseDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int DurationInMinutes { get; set; }
    public decimal PriceInCents { get; set; }
    public string BusinessId { get; set; }
}
