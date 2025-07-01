using Domain.Entities;
using Domain.Interfaces.Services.DbConfig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Service;

public interface IServiceProviderService : IServiceBaseConfig<tb_service_provider>
{
    Task<List<tb_service_provider>> GetAllServiceProvidersByBusinessIdAsync(Guid businessId);
    Task<tb_service_provider> GetServiceProviderByIdAsync(Guid serviceProviderId);
    Task<tb_service_provider> CreateServiceProviderAsync(tb_service_provider serviceProvider);
    Task<bool> UpdateServiceProviderAsync(tb_service_provider serviceProvider);
    Task<bool> DeleteServiceProviderAsync(Guid serviceProviderId);
    Task<bool> IsServiceProviderOwnedByBusinessOwnerAsync(Guid serviceProviderId, string userId);
    Task<bool> IsServiceProviderLinkedToUserAsync(Guid serviceProviderId, string userId); // If a service provider account corresponds to a specific user
}