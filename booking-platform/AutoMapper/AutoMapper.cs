using AutoMapper;
using booking_platform.DTO;
using Domain.Entities;

namespace booking_platform.AutoMapper;

public class AutoMapper : Profile
{
    public AutoMapper()
    {
        _ = CreateMap<tb_appointments, AppointmentDTO>();
        _ = CreateMap<tb_business, BusinessRequestDTO>();
        _ = CreateMap<tb_service, ServiceDTO>();
        _ = CreateMap<tb_service_provider, ServiceProviderDTO>();
        _ = CreateMap<ApplicationUser, UserDTO>();
    }
}
