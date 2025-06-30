using Domain.Entities;
using Domain.Interfaces.Repositories.BaseConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories;

public interface IServiceRepository : IRepositoryBaseConfig<tb_service>
{
    Task<tb_service> GetByIdAsync(Guid id);
    Task<List<tb_service>> GetAllServicesByBusinessIdAsync(Guid businessId);
}
