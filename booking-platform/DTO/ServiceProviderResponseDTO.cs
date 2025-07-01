using System;

namespace booking_platform.DTO;

public class ServiceProviderResponseDTO
{
    public string Id { get; set; }
    public string BusinessId { get; set; }
    public string? UserId { get; set; }
    public string Name { get; set; }
    public string? AvatarImageUrl { get; set; }
    public int AvailableFromWeekday { get; set; }
    public int AvailableToWeekday { get; set; }
    public TimeSpan AvailableFromTime { get; set; }
    public TimeSpan AvailableToTime { get; set; }
}