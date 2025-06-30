using Domain.Entities;
using Domain.Interfaces.Services.DbConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Service;

public interface IServiceService : IServiceBaseConfig<tb_service>
{
    Task<List<tb_service>> GetAllServicesByBusinessIdAsync(Guid businessId);
    Task<tb_service> GetServiceByIdAsync(Guid serviceId);
    Task<tb_service> CreateServiceAsync(tb_service service);
    Task<bool> UpdateServiceAsync(tb_service service);
    Task<bool> DeleteServiceAsync(Guid serviceId);
    Task<bool> IsServiceOwnedByUserAsync(Guid serviceId, string userId);
}
