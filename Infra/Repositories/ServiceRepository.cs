using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.Context;
using Infra.Repository.DbConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Repositories
{
    public class ServiceRepository : RepositoryBaseConfig<tb_service>, IServiceRepository
    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<tb_service>> GetAllServicesByBusinessIdAsync(Guid businessId)
        {
            return await _context.Set<tb_service>()
                                 .Where(s => s.business_id == businessId)
                                 .Include(s => s.ServiceProviderServices)
                                    .ThenInclude(sps => sps.service_provider)
                                 .Include(s => s.Appointments)
                                 .ToListAsync();
        }

        public async Task<tb_service> GetByIdAsync(Guid id)
        {
            return await _context.Set<tb_service>()
                                 .Where(s => s.id == id)
                                 .Include(s => s.ServiceProviderServices)
                                    .ThenInclude(sps => sps.service_provider)
                                 .Include(s => s.Appointments)
                                 .FirstOrDefaultAsync();
        }
    }
}
