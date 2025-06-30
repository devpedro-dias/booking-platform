using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.Context;
using Infra.Repository.DbConfig;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class BusinessRepository : RepositoryBaseConfig<tb_business>, IBusinessRepository
{
    private readonly AppDbContext _context;

    public BusinessRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<tb_business>> GetAllBusinessesByUserIdWithDetailsAsync(string userId)
    {
        return await _context.Set<tb_business>()
                             .Include(b => b.owner_user)
                             .Include(b => b.Services)
                             .Include(b => b.ServiceProviders)
                             .AsNoTracking()
                             .Where(b => b.owner_user_id == userId)
                             .ToListAsync();
    }

    public async Task<tb_business> GetByIdWithDetailsAsync(Guid businessId)
    {
        return await _context.Set<tb_business>()
                             .Include(b => b.owner_user)
                             .Include(b => b.Services)
                             .Include(b => b.ServiceProviders)
                             .AsNoTracking()
                             .FirstOrDefaultAsync(b => b.id == businessId);
    }

    public async Task<bool> ExistsBusinessNameForUserAsync(string businessName, string userId, Guid? excludeBusinessId = null)
    {
        var query = _context.Set<tb_business>()
                             .Where(b => b.name.ToUpper() == businessName.ToUpper() && b.owner_user_id == userId);

        if (excludeBusinessId.HasValue)
        {
            query = query.Where(b => b.id != excludeBusinessId.Value);
        }

        return await query.AnyAsync();
    }
}