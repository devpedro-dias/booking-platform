using booking_platform.DTO;
using Domain.Entities;
using Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace booking_platform.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly IBusinessService _businessService;
    private readonly IServiceProviderService _serviceProviderService;
    private readonly IServiceService _serviceService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AppointmentController(
        IAppointmentService appointmentService,
        IBusinessService businessService,
        IServiceProviderService serviceProviderService,
        IServiceService serviceService,
        UserManager<ApplicationUser> userManager)
    {
        _appointmentService = appointmentService;
        _businessService = businessService;
        _serviceProviderService = serviceProviderService;
        _serviceService = serviceService;
        _userManager = userManager;
    }

    private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private AppointmentResponseDTO MapAppointmentToDto(tb_appointments appointment)
    {
        return new AppointmentResponseDTO
        {
            Id = appointment.id.ToString(),
            StartDateTime = appointment.start_date_time,
            EndDateTime = appointment.end_date_time,
            TotalPriceInCents = appointment.total_price_in_cents,
            Status = appointment.status,
            CreatedAt = appointment.created_at,
            BusinessId = appointment.business_id.ToString(),
            CustomerId = appointment.customer_id,
            ServiceProviderId = appointment.service_provider_id.ToString(),
            ServiceId = appointment.service_id.ToString()
        };
    }

    [HttpGet("my-appointments")]
    public async Task<ActionResult<List<AppointmentResponseDTO>>> GetMyAppointments()
    {
        var customerId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(customerId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        try
        {
            var appointments = await _appointmentService.GetCustomerAppointmentsAsync(customerId);
            return Ok(appointments.Select(MapAppointmentToDto).ToList());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while retrieving your appointments.", error = ex.Message });
        }
    }

    [HttpPost("book")]
    public async Task<ActionResult<AppointmentResponseDTO>> BookAppointment([FromBody] AppointmentRequestDTO appointmentDto)
    {
        var customerId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(customerId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (appointmentDto.CustomerId != customerId)
        {
            return Forbid("You can only book appointments for yourself.");
        }

        if (appointmentDto.StartDateTime >= appointmentDto.EndDateTime)
        {
            return BadRequest("Start date/time must be before end date/time.");
        }
        if (appointmentDto.StartDateTime < DateTime.UtcNow.AddMinutes(5))
        {
            return BadRequest("Appointments must be booked for a future time.");
        }

        try
        {
            var service = await _serviceService.GetByIdAsync(appointmentDto.ServiceId);
            if (service == null)
            {
                return BadRequest("Invalid Service ID.");
            }

            var expectedEndTime = appointmentDto.StartDateTime.AddMinutes(service.duration_in_minutes);
            if (appointmentDto.EndDateTime != expectedEndTime)
            {
                return BadRequest($"Appointment end time must be {service.duration_in_minutes} minutes after start time for this service. Expected: {expectedEndTime:HH:mm}.");
            }

            var newAppointment = new tb_appointments
            {
                id = Guid.NewGuid(),
                start_date_time = appointmentDto.StartDateTime,
                end_date_time = appointmentDto.EndDateTime,
                status = "pending",
                business_id = appointmentDto.BusinessId,
                customer_id = customerId, 
                service_provider_id = appointmentDto.ServiceProviderId,
                service_id = appointmentDto.ServiceId,
                total_price_in_cents = service.price_in_cents 
            };

            var createdAppointment = await _appointmentService.CreateAppointmentForCustomerAsync(newAppointment);
            return CreatedAtAction(nameof(GetMyAppointments), new { id = createdAppointment.id }, MapAppointmentToDto(createdAppointment));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while booking the appointment.", error = ex.Message });
        }
    }

    [HttpPut("cancel/{id}")]
    public async Task<IActionResult> CancelAppointment(Guid id)
    {
        var customerId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(customerId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        try
        {
            var success = await _appointmentService.CancelAppointmentByCustomerAsync(id, customerId);
            if (!success)
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null) return NotFound("Appointment not found.");
                if (appointment.customer_id != customerId) return Forbid("You do not have permission to cancel this appointment.");
                return StatusCode(500, new { success = false, message = "Failed to cancel appointment (unexpected error)." });
            }
            return Ok(new { success = true, message = "Appointment cancelled successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while cancelling the appointment.", error = ex.Message });
        }
    }

    [HttpGet("business/{businessId}")]
    public async Task<ActionResult<List<AppointmentResponseDTO>>> GetBusinessAppointments(Guid businessId)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        var business = await _businessService.GetBusinessByIdAsync(businessId);
        if (business == null || business.owner_user_id != userId)
        {
            return NotFound("Business not found or you do not have permission to view its appointments.");
        }

        try
        {
            var appointments = await _appointmentService.GetAllAppointmentsByBusinessIdAsync(businessId);
            return Ok(appointments.Select(MapAppointmentToDto).ToList());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while retrieving business appointments.", error = ex.Message });
        }
    }

    [HttpGet("provider/{serviceProviderId}")]
    public async Task<ActionResult<List<AppointmentResponseDTO>>> GetServiceProviderAppointments(Guid serviceProviderId)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        var serviceProvider = await _serviceProviderService.GetServiceProviderByIdAsync(serviceProviderId);
        if (serviceProvider == null)
        {
            return NotFound("Service Provider not found.");
        }

        var business = await _businessService.GetBusinessByIdAsync(serviceProvider.business_id);
        if (business == null || business.owner_user_id != userId)
        {
            return Forbid("You do not have permission to view appointments for this service provider.");
        }

        try
        {
            var appointments = await _appointmentService.GetAppointmentsByServiceProviderIdAsync(serviceProviderId);
            return Ok(appointments.Select(MapAppointmentToDto).ToList());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while retrieving service provider appointments.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] AppointmentRequestDTO appointmentDto)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
        if (existingAppointment == null)
        {
            return NotFound("Appointment not found.");
        }

        var business = await _businessService.GetBusinessByIdAsync(existingAppointment.business_id);
        if (business == null || business.owner_user_id != userId)
        {
            return Forbid("You do not have permission to update this appointment.");
        }

        if (existingAppointment.business_id != appointmentDto.BusinessId)
        {
            var newBusiness = await _businessService.GetBusinessByIdAsync(appointmentDto.BusinessId);
            if (newBusiness == null || newBusiness.owner_user_id != userId)
            {
                return Forbid("You do not have permission to move this appointment to the specified business.");
            }
        }
        if (existingAppointment.service_provider_id != appointmentDto.ServiceProviderId)
        {
            var newServiceProvider = await _serviceProviderService.GetServiceProviderByIdAsync(appointmentDto.ServiceProviderId);
            if (newServiceProvider == null || newServiceProvider.business_id != appointmentDto.BusinessId)
            {
                return BadRequest("New Service Provider is invalid or does not belong to the specified Business.");
            }
        }
        if (existingAppointment.service_id != appointmentDto.ServiceId)
        {
            var newService = await _serviceService.GetByIdAsync(appointmentDto.ServiceId);
            if (newService == null || newService.business_id != appointmentDto.BusinessId)
            {
                return BadRequest("New Service is invalid or does not belong to the specified Business.");
            }
        }
        if (existingAppointment.customer_id != appointmentDto.CustomerId)
        {
            var newCustomer = await _userManager.FindByIdAsync(appointmentDto.CustomerId);
            if (newCustomer == null)
            {
                return BadRequest("New Customer ID does not exist.");
            }
        }

        if (appointmentDto.StartDateTime >= appointmentDto.EndDateTime)
        {
            return BadRequest("Start date/time must be before end date/time.");
        }

        try
        {
            existingAppointment.start_date_time = appointmentDto.StartDateTime;
            existingAppointment.end_date_time = appointmentDto.EndDateTime;
            existingAppointment.status = appointmentDto.Status;
            existingAppointment.business_id = appointmentDto.BusinessId;
            existingAppointment.customer_id = appointmentDto.CustomerId;
            existingAppointment.service_provider_id = appointmentDto.ServiceProviderId;
            existingAppointment.service_id = appointmentDto.ServiceId;

            if (existingAppointment.service_id != appointmentDto.ServiceId)
            {
                var newService = await _serviceService.GetByIdAsync(appointmentDto.ServiceId);
                existingAppointment.total_price_in_cents = newService?.price_in_cents ?? 0;
            }

            var success = await _appointmentService.UpdateAppointmentByBusinessOwnerAsync(existingAppointment);
            if (!success)
            {
                return StatusCode(500, new { success = false, message = "Failed to update appointment (unexpected error)." });
            }
            return Ok(new { success = true, message = "Appointment updated successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while updating the appointment.", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(Guid id)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        try
        {
            var appointmentToDelete = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointmentToDelete == null)
            {
                return NotFound("Appointment not found.");
            }

            var business = await _businessService.GetBusinessByIdAsync(appointmentToDelete.business_id);
            if (business == null || business.owner_user_id != userId)
            {
                return Forbid("You do not have permission to delete this appointment.");
            }

            var success = await _appointmentService.DeleteAppointmentByBusinessOwnerAsync(id);
            if (!success)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete appointment (unexpected error)." });
            }
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            string errorMessage = "An error occurred while deleting the appointment.";
            if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_"))
            {
                errorMessage = "Cannot delete the appointment as it is linked to other entities.";
            }
            return StatusCode(409, new { success = false, message = errorMessage, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while deleting the appointment.", error = ex.Message });
        }
    }
}