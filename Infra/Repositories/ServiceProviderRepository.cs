using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.Context;
using Infra.Repository.DbConfig;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories;

public class ServiceProviderRepository : RepositoryBaseConfig<tb_service_provider>, IServiceProviderRepository
{
    private readonly AppDbContext _context;

    public ServiceProviderRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<tb_service_provider>> GetAllServiceProvidersByBusinessIdAsync(Guid businessId)
    {
        return await _context.Set<tb_service_provider>()
                             .Where(sp => sp.business_id == businessId)
                             .Include(sp => sp.business)
                             .Include(sp => sp.user)
                             .Include(sp => sp.Appointments)
                             .Include(sp => sp.ServiceProviderServices)
                                .ThenInclude(sps => sps.service)
                             .ToListAsync();
    }

    public async Task<tb_service_provider> GetByIdAsync(Guid id)
    {
        return await _context.Set<tb_service_provider>()
                             .Where(sp => sp.id == id)
                             .Include(sp => sp.business)
                             .Include(sp => sp.user)
                             .Include(sp => sp.Appointments)
                             .Include(sp => sp.ServiceProviderServices)
                                .ThenInclude(sps => sps.service)
                             .FirstOrDefaultAsync();
    }
}