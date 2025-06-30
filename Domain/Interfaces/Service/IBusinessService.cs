using Domain.Entities;
using Domain.Interfaces.Services.DbConfig;

namespace Domain.Interfaces.Service;

public interface IBusinessService : IServiceBaseConfig<tb_business>
{
    Task<List<tb_business>> GetAllBusinessesByUserIdAsync(string userId);
    Task<tb_business> GetBusinessByIdAsync(Guid businessId);
    Task<tb_business> CreateBusinessAsync(tb_business business);
    Task<bool> UpdateBusinessAsync(tb_business business);
    Task<bool> DeleteBusinessAsync(Guid businessId);
    Task<bool> BusinessNameExistsForUserAsync(string businessName, string userId, Guid? excludeBusinessId = null);
}

