using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infra.Context;
using Infra.Repository.DbConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Repositories;

public class AppointmentRepository : RepositoryBaseConfig<tb_appointments>, IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    private IQueryable<tb_appointments> IncludeRelatedEntities(IQueryable<tb_appointments> query)
    {
        return query
            .Include(a => a.business)
            .Include(a => a.customer)
            .Include(a => a.service_provider)
            .Include(a => a.service);
    }

    public async Task<List<tb_appointments>> GetCustomerAppointmentsAsync(string customerId)
    {
        return await IncludeRelatedEntities(_context.Set<tb_appointments>())
            .Where(a => a.customer_id == customerId)
            .OrderByDescending(a => a.start_date_time)
            .ToListAsync();
    }

    public async Task<List<tb_appointments>> GetAllAppointmentsByBusinessIdAsync(Guid businessId)
    {
        return await IncludeRelatedEntities(_context.Set<tb_appointments>())
            .Where(a => a.business_id == businessId)
            .OrderBy(a => a.start_date_time)
            .ToListAsync();
    }

    public async Task<List<tb_appointments>> GetAppointmentsByServiceProviderIdAsync(Guid serviceProviderId)
    {
        return await IncludeRelatedEntities(_context.Set<tb_appointments>())
            .Where(a => a.service_provider_id == serviceProviderId)
            .OrderBy(a => a.start_date_time)
            .ToListAsync();
    }

    public async Task<tb_appointments> GetByIdAsync(Guid id)
    {
        return await IncludeRelatedEntities(_context.Set<tb_appointments>())
            .FirstOrDefaultAsync(a => a.id == id);
    }

    public async Task<bool> HasOverlappingAppointmentsAsync(Guid serviceProviderId, DateTime startDateTime, DateTime endDateTime, Guid? excludeAppointmentId = null)
    {
        return await _context.Set<tb_appointments>()
            .AnyAsync(a =>
                a.service_provider_id == serviceProviderId &&
                a.status != "cancelled" &&
                (
                    (startDateTime < a.end_date_time && endDateTime > a.start_date_time)
                ) &&
                (excludeAppointmentId == null || a.id != excludeAppointmentId.Value)
            );
    }
}