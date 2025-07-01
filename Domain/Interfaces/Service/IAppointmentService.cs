using Domain.Entities;
using Domain.Interfaces.Services.DbConfig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces.Service;

public interface IAppointmentService : IServiceBaseConfig<tb_appointments>
{
    // Customer-facing methods
    Task<List<tb_appointments>> GetCustomerAppointmentsAsync(string customerId);
    Task<tb_appointments> CreateAppointmentForCustomerAsync(tb_appointments appointment);
    Task<bool> CancelAppointmentByCustomerAsync(Guid appointmentId, string customerId); // Specific logic for customer cancellation

    // Business-owner facing methods
    Task<List<tb_appointments>> GetAllAppointmentsByBusinessIdAsync(Guid businessId);
    Task<List<tb_appointments>> GetAppointmentsByServiceProviderIdAsync(Guid serviceProviderId);
    Task<tb_appointments> GetAppointmentByIdAsync(Guid appointmentId);
    Task<bool> UpdateAppointmentByBusinessOwnerAsync(tb_appointments appointment); // Specific logic for business owner update
    Task<bool> DeleteAppointmentByBusinessOwnerAsync(Guid appointmentId); // Specific logic for business owner delete

    // Core validation methods (might be private/protected in implementation, but useful for understanding)
    Task<bool> IsTimeSlotAvailableAsync(Guid serviceProviderId, DateTime startDateTime, DateTime endDateTime, Guid? excludeAppointmentId = null);
    Task<bool> IsServiceProviderAvailableAsync(Guid serviceProviderId, DateTime startDateTime, DateTime endDateTime);
    Task<decimal> CalculateAppointmentPriceAsync(Guid serviceId);
    Task<int> GetServiceDurationMinutesAsync(Guid serviceId); // Helper to get service duration
}