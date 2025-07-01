using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Service;
using Domain.Services.DbConfig;
using Microsoft.AspNetCore.Identity;

namespace Domain.Services;

public class AppointmentService : ServiceBaseConfig<tb_appointments>, IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IBusinessRepository _businessRepository;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IBusinessRepository businessRepository,
        IServiceProviderRepository serviceProviderRepository,
        IServiceRepository serviceRepository,
        UserManager<ApplicationUser> userManager
    ) : base(appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
        _businessRepository = businessRepository;
        _serviceProviderRepository = serviceProviderRepository;
        _serviceRepository = serviceRepository;
        _userManager = userManager;
    }

    private async Task<(tb_business business, tb_service_provider serviceProvider, tb_service service, ApplicationUser customer, string errorMessage)> ValidateRelatedEntities(tb_appointments appointment)
    {
        var business = await _businessRepository.GetByIdAsync(appointment.business_id);
        if (business == null) return (null, null, null, null, "Business not found.");

        var serviceProvider = await _serviceProviderRepository.GetByIdAsync(appointment.service_provider_id);
        if (serviceProvider == null) return (null, null, null, null, "Service provider not found.");

        var service = await _serviceRepository.GetByIdAsync(appointment.service_id);
        if (service == null) return (null, null, null, null, "Service not found.");

        var customer = await _userManager.FindByIdAsync(appointment.customer_id);
        if (customer == null) return (null, null, null, null, "Customer not found.");

        // Check if service provider belongs to the business
        if (serviceProvider.business_id != appointment.business_id)
            return (null, null, null, null, "Service provider does not belong to the specified business.");

        // Check if service belongs to the business
        if (service.business_id != appointment.business_id)
            return (null, null, null, null, "Service does not belong to the specified business.");

        // Check if appointment duration matches service duration
        // This is a common rule, if you need it, uncomment and add to your DTO/Logic
        // var expectedDuration = TimeSpan.FromMinutes(service.duration_in_minutes);
        // if ((appointment.end_date_time - appointment.start_date_time) != expectedDuration)
        //     return (null, null, null, null, "Appointment duration must match service duration.");

        return (business, serviceProvider, service, customer, null); // All good
    }

    // Core validation: Is the specific time slot available for this provider?
    public async Task<bool> IsTimeSlotAvailableAsync(Guid serviceProviderId, DateTime startDateTime, DateTime endDateTime, Guid? excludeAppointmentId = null)
    {
        return await _appointmentRepository.HasOverlappingAppointmentsAsync(
            serviceProviderId,
            startDateTime,
            endDateTime,
            excludeAppointmentId
        );
    }

    // Core validation: Is the service provider available within their defined hours?
    public async Task<bool> IsServiceProviderAvailableAsync(Guid serviceProviderId, DateTime startDateTime, DateTime endDateTime)
    {
        var serviceProvider = await _serviceProviderRepository.GetByIdAsync(serviceProviderId);
        if (serviceProvider == null) return false;

        int appointmentDayOfWeek = (int)startDateTime.DayOfWeek;

        if (appointmentDayOfWeek < serviceProvider.available_from_weekday ||
            appointmentDayOfWeek > serviceProvider.available_to_weekday)
        {
            return false;
        }

        var appointmentStartTime = startDateTime.TimeOfDay;
        var appointmentEndTime = endDateTime.TimeOfDay;

        if (appointmentStartTime < serviceProvider.available_from_time ||
            appointmentEndTime > serviceProvider.available_to_time)
        {
            return false;
        }

        // Optionally, ensure the appointment does not span across midnight if availability is restricted to a single day
        // if (serviceProvider.available_from_time > serviceProvider.available_to_time &&
        //     (appointmentStartTime < serviceProvider.available_from_time && appointmentEndTime > serviceProvider.available_to_time))
        // {
        //     return false; // Assuming basic same-day availability for this check
        // }

        return true;
    }

    public async Task<decimal> CalculateAppointmentPriceAsync(Guid serviceId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service == null) return 0;
        return service.price_in_cents;
    }

    public async Task<int> GetServiceDurationMinutesAsync(Guid serviceId)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId);
        if (service == null) return 0;
        return service.duration_in_minutes;
    }

    public async Task<List<tb_appointments>> GetCustomerAppointmentsAsync(string customerId)
    {
        return await _appointmentRepository.GetCustomerAppointmentsAsync(customerId);
    }

    public async Task<tb_appointments> CreateAppointmentForCustomerAsync(tb_appointments appointment)
    {
        var (business, serviceProvider, service, customer, errorMessage) = await ValidateRelatedEntities(appointment);
        if (errorMessage != null) throw new ArgumentException(errorMessage);

        if (!await IsServiceProviderAvailableAsync(appointment.service_provider_id, appointment.start_date_time, appointment.end_date_time))
        {
            throw new InvalidOperationException("Service provider is not available during the requested time slot.");
        }

        if (!await IsTimeSlotAvailableAsync(appointment.service_provider_id, appointment.start_date_time, appointment.end_date_time))
        {
            throw new InvalidOperationException("The service provider has another appointment scheduled for the requested time slot.");
        }

        appointment.total_price_in_cents = service.price_in_cents;

        if (string.IsNullOrEmpty(appointment.status))
        {
            appointment.status = "pending";
        }

        appointment.created_at = DateTime.UtcNow;

        return await _appointmentRepository.AddAsync(appointment);
    }

    public async Task<bool> CancelAppointmentByCustomerAsync(Guid appointmentId, string customerId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment == null || appointment.customer_id != customerId)
        {
            return false;
        }

        // Add logic for when cancellation is allowed (e.g., cannot cancel within X hours)
        if (appointment.start_date_time - DateTime.UtcNow < TimeSpan.FromHours(6))
        {
            throw new InvalidOperationException("Cannot cancel appointment within 6 hours of start time.");
        }

        if (appointment.status == "cancelled" || appointment.status == "completed")
        {
            throw new InvalidOperationException($"Appointment is already in '{appointment.status}' status and cannot be cancelled.");
        }

        appointment.status = "cancelled";
        return await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task<List<tb_appointments>> GetAllAppointmentsByBusinessIdAsync(Guid businessId)
    {
        return await _appointmentRepository.GetAllAppointmentsByBusinessIdAsync(businessId);
    }

    public async Task<List<tb_appointments>> GetAppointmentsByServiceProviderIdAsync(Guid serviceProviderId)
    {
        return await _appointmentRepository.GetAppointmentsByServiceProviderIdAsync(serviceProviderId);
    }

    public async Task<tb_appointments> GetAppointmentByIdAsync(Guid appointmentId)
    {
        return await _appointmentRepository.GetByIdAsync(appointmentId);
    }

    public async Task<bool> UpdateAppointmentByBusinessOwnerAsync(tb_appointments appointment)
    {
        var existingAppointment = await _appointmentRepository.GetByIdAsync(appointment.id);
        if (existingAppointment == null) return false;

        var (business, serviceProvider, service, customer, errorMessage) = await ValidateRelatedEntities(appointment);
        if (errorMessage != null) throw new ArgumentException(errorMessage);

        if (existingAppointment.service_provider_id != appointment.service_provider_id ||
            existingAppointment.start_date_time != appointment.start_date_time ||
            existingAppointment.end_date_time != appointment.end_date_time)
        {
            if (!await IsServiceProviderAvailableAsync(appointment.service_provider_id, appointment.start_date_time, appointment.end_date_time))
            {
                throw new InvalidOperationException("Service provider is not available during the requested time slot.");
            }

            if (!await IsTimeSlotAvailableAsync(appointment.service_provider_id, appointment.start_date_time, appointment.end_date_time, appointment.id))
            {
                throw new InvalidOperationException("The service provider has another appointment scheduled for the requested time slot.");
            }
        }

        existingAppointment.start_date_time = appointment.start_date_time;
        existingAppointment.end_date_time = appointment.end_date_time;
        existingAppointment.total_price_in_cents = service.price_in_cents;
        existingAppointment.status = appointment.status;
        existingAppointment.business_id = appointment.business_id;
        existingAppointment.customer_id = appointment.customer_id;
        existingAppointment.service_provider_id = appointment.service_provider_id;
        existingAppointment.service_id = appointment.service_id;

        return await _appointmentRepository.UpdateAsync(existingAppointment);
    }

    public async Task<bool> DeleteAppointmentByBusinessOwnerAsync(Guid appointmentId)
    {
        var appointmentToDelete = await _businessRepository.GetByIdAsync(appointmentId);
        if (appointmentToDelete == null)
        {
            return false;
        }
        await _businessRepository.DeleteAsync(appointmentToDelete);
        return true;
    }
}