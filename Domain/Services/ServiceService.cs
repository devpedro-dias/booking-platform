using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Service;
using Domain.Services.DbConfig;

namespace Domain.Services;

public class ServiceService : ServiceBaseConfig<tb_service>, IServiceService
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IBusinessRepository _businessRepository;

    public ServiceService(IServiceRepository serviceRepository, IBusinessRepository businessRepository)
        : base(serviceRepository)
    {
        _serviceRepository = serviceRepository;
        _businessRepository = businessRepository;
    }

    public async Task<List<tb_service>> GetAllServicesByBusinessIdAsync(Guid businessId)
    {
        return await _serviceRepository.GetAllServicesByBusinessIdAsync(businessId);
    }

    public async Task<tb_service> GetServiceByIdAsync(Guid serviceId)
    {
        return await _serviceRepository.GetByIdAsync(serviceId);
    }

    public async Task<tb_service> CreateServiceAsync(tb_service service)
    {
        return await _serviceRepository.AddAsync(service);
    }

    public async Task<bool> UpdateServiceAsync(tb_service service)
    {
        return await _serviceRepository.UpdateAsync(service);
    }

    public async Task<bool> DeleteServiceAsync(Guid serviceId)
    {
        var serviceToDelete = await _serviceRepository.GetByIdAsync(serviceId);
        if (serviceToDelete == null)
        {
            return false;
        }
        await _serviceRepository.DeleteAsync(serviceToDelete);
        return true;
    }

    public async Task<bool> IsServiceOwnedByUserAsync(Guid serviceId, string userId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service == null)
        {
            return false;
        }

        var business = await _businessRepository.GetByIdAsync(service.business_id);
        return business != null && business.owner_user_id == userId;
    }
}