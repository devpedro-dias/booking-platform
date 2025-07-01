using Domain.Entities;
using Domain.Interfaces.Repositories.BaseConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories;

public interface IServiceProviderRepository : IRepositoryBaseConfig<tb_service_provider>
{
    Task<tb_service_provider> GetByIdAsync(Guid id);
    Task<List<tb_service_provider>> GetAllServiceProvidersByBusinessIdAsync(Guid businessId);
}
