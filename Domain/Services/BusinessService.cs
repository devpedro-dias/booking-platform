using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Service;
using Domain.Services.DbConfig;

namespace Domain.Services;

public class BusinessService : ServiceBaseConfig<tb_business>, IBusinessService
{
    protected readonly IBusinessRepository _businessRepository;

    public BusinessService(IBusinessRepository businessRepository)
        : base(businessRepository)
    {
        _businessRepository = businessRepository;
    }

    public async Task<List<tb_business>> GetAllBusinessesByUserIdAsync(string userId)
    {
        var allBusinesses = await _businessRepository.GetAllAsync();
        return allBusinesses.Where(b => b.owner_user_id == userId).ToList();
    }

    public async Task<tb_business> GetBusinessByIdAsync(Guid businessId)
    {
        return await _businessRepository.GetByIdAsync(businessId);
    }

    public async Task<tb_business> CreateBusinessAsync(tb_business business)
    {
        business.id = Guid.NewGuid();
        business.name = business.name.Trim();
        business.address = business.address.Trim();
        business.phone_number = business.phone_number.Trim();

        await _businessRepository.AddAsync(business);
        return business;
    }

    public async Task<bool> UpdateBusinessAsync(tb_business businessToUpdate)
    {
        var existingBusiness = await _businessRepository.GetByIdAsync(businessToUpdate.id);

        if (existingBusiness == null)
        {
            return false;
        }

        existingBusiness.name = businessToUpdate.name.Trim();
        existingBusiness.address = businessToUpdate.address.Trim();
        existingBusiness.phone_number = businessToUpdate.phone_number.Trim();

        await _businessRepository.UpdateAsync(existingBusiness);
        return true;
    }

    public async Task<bool> DeleteBusinessAsync(Guid businessId)
    {
        var businessToDelete = await _businessRepository.GetByIdAsync(businessId);
        if (businessToDelete == null)
        {
            return false;
        }
        await _businessRepository.DeleteAsync(businessToDelete);
        return true;
    }

    public async Task<bool> BusinessNameExistsForUserAsync(string businessName, string userId, Guid? excludeBusinessId = null)
    {
        var allBusinesses = await _businessRepository.GetAllAsync(); 

        var query = allBusinesses
                        .Where(b => b.name.ToUpper() == businessName.ToUpper() && b.owner_user_id == userId);

        if (excludeBusinessId.HasValue)
        {
            query = query.Where(b => b.id != excludeBusinessId.Value);
        }

        return query.Any();
    }
}