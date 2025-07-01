using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Service;
using Domain.Services.DbConfig;

namespace Domain.Services;

public class ServiceProviderService : ServiceBaseConfig<tb_service_provider>, IServiceProviderService
{
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly IBusinessRepository _businessRepository;

    public ServiceProviderService(
        IServiceProviderRepository serviceProviderRepository,
        IBusinessRepository businessRepository
    ) : base(serviceProviderRepository)
    {
        _serviceProviderRepository = serviceProviderRepository;
        _businessRepository = businessRepository;
    }

    public async Task<List<tb_service_provider>> GetAllServiceProvidersByBusinessIdAsync(Guid businessId)
    {
        return await _serviceProviderRepository.GetAllServiceProvidersByBusinessIdAsync(businessId);
    }

    public async Task<tb_service_provider> GetServiceProviderByIdAsync(Guid serviceProviderId)
    {
        return await _serviceProviderRepository.GetByIdAsync(serviceProviderId);
    }

    public async Task<tb_service_provider> CreateServiceProviderAsync(tb_service_provider serviceProvider)
    {
        return await _serviceProviderRepository.AddAsync(serviceProvider);
    }

    public async Task<bool> UpdateServiceProviderAsync(tb_service_provider serviceProvider)
    {
        return await _serviceProviderRepository.UpdateAsync(serviceProvider);
    }

    public async Task<bool> DeleteServiceProviderAsync(Guid serviceProviderId)
    {
        var serviceProviderToDelete = await _businessRepository.GetByIdAsync(serviceProviderId);
        if (serviceProviderToDelete == null)
        {
            return false;
        }
        await _businessRepository.DeleteAsync(serviceProviderToDelete);
        return true;
    }

    public async Task<bool> IsServiceProviderOwnedByBusinessOwnerAsync(Guid serviceProviderId, string userId)
    {
        var serviceProvider = await _serviceProviderRepository.GetByIdAsync(serviceProviderId);
        if (serviceProvider == null)
        {
            return false;
        }

        var business = await _businessRepository.GetByIdAsync(serviceProvider.business_id);
        return business != null && business.owner_user_id == userId;
    }

    public async Task<bool> IsServiceProviderLinkedToUserAsync(Guid serviceProviderId, string userId)
    {
        var serviceProvider = await _serviceProviderRepository.GetByIdAsync(serviceProviderId);
        return serviceProvider != null && serviceProvider.user_id == userId;
    }
}