using Domain.Entities;
using Domain.Interfaces.Repositories.BaseConfig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Repositories;

public interface IAppointmentRepository : IRepositoryBaseConfig<tb_appointments>
{
    Task<List<tb_appointments>> GetCustomerAppointmentsAsync(string customerId);
    Task<List<tb_appointments>> GetAllAppointmentsByBusinessIdAsync(Guid businessId);
    Task<List<tb_appointments>> GetAppointmentsByServiceProviderIdAsync(Guid serviceProviderId);
    Task<bool> HasOverlappingAppointmentsAsync(Guid serviceProviderId, DateTime startDateTime, DateTime endDateTime, Guid? excludeAppointmentId = null);
}