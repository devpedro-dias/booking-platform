namespace booking_platform.DTO;

public class BusinessResponseDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string OwnerUserId { get; set; }
    public ICollection<ServiceResponseDTO> Services { get; set; }
    public ICollection<ServiceProviderDTO> ServiceProviders { get; set; }
}