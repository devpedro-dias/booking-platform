using booking_platform.DTO;
using Domain.Entities;
using Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; 

namespace booking_platform.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class ServiceController : ControllerBase
{
    private readonly IServiceService _serviceService;
    private readonly IBusinessService _businessService;

    public ServiceController(IServiceService serviceService, IBusinessService businessService)
    {
        _serviceService = serviceService;
        _businessService = businessService;
    }

    private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private ServiceResponseDTO MapServiceToDto(tb_service service)
    {
        return new ServiceResponseDTO
        {
            Id = service.id.ToString(),
            Name = service.name,
            Description = service.description,
            PriceInCents = service.price_in_cents,
            DurationInMinutes = service.duration_in_minutes,
            BusinessId = service.business_id.ToString()
        };
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceResponseDTO>>> Get([FromQuery] Guid businessId)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        var business = await _businessService.GetBusinessByIdAsync(businessId);
        if (business == null || business.owner_user_id != userId)
        {
            return NotFound("Business not found or you do not have permission to view its services.");
        }

        try
        {
            var services = await _serviceService.GetAllServicesByBusinessIdAsync(businessId);
            var responseDtos = services.Select(MapServiceToDto).ToList();
            return Ok(responseDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while retrieving services.", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponseDTO>> GetById(Guid id)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        var service = await _serviceService.GetServiceByIdAsync(id);

        if (service == null)
        {
            return NotFound("Service not found.");
        }

        var business = await _businessService.GetBusinessByIdAsync(service.business_id);
        if (business == null || business.owner_user_id != userId)
        {
            return Forbid("You do not have permission to view this service.");
        }

        return Ok(MapServiceToDto(service));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponseDTO>> Create([FromBody] ServiceRequestDTO serviceDto)
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

        var business = await _businessService.GetBusinessByIdAsync(serviceDto.BusinessId);
        if (business == null || business.owner_user_id != userId)
        {
            return NotFound("Business not found or you do not have permission to add services to it.");
        }

        try
        {
            var service = new tb_service
            {
                id = Guid.NewGuid(),
                name = serviceDto.Name.Trim(),
                description = serviceDto.Description.Trim(),
                price_in_cents = serviceDto.PriceInCents,
                duration_in_minutes = serviceDto.DurationInMinutes,
                business_id = serviceDto.BusinessId
            };

            var createdService = await _serviceService.CreateServiceAsync(service);

            var responseDto = MapServiceToDto(createdService);

            return CreatedAtAction(nameof(GetById), new { id = createdService.id }, responseDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while creating the service.", error = ex.Message });
        }
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] ServiceRequestDTO serviceDto)
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

        try
        {
            var existingService = await _serviceService.GetServiceByIdAsync(id);
            if (existingService == null)
            {
                return NotFound(new { success = false, message = "Service not found." });
            }

            var business = await _businessService.GetBusinessByIdAsync(existingService.business_id);
            if (business == null || business.owner_user_id != userId)
            {
                return Forbid("You do not have permission to update this service.");
            }

            if (existingService.business_id != serviceDto.BusinessId)
            {
                var newBusiness = await _businessService.GetBusinessByIdAsync(serviceDto.BusinessId);
                if (newBusiness == null || newBusiness.owner_user_id != userId)
                {
                    return Forbid("You do not have permission to move this service to the specified business.");
                }
            }

            existingService.name = serviceDto.Name.Trim();
            existingService.description = serviceDto.Description.Trim();
            existingService.price_in_cents = serviceDto.PriceInCents;
            existingService.duration_in_minutes = serviceDto.DurationInMinutes;
            existingService.business_id = serviceDto.BusinessId;

            var success = await _serviceService.UpdateServiceAsync(existingService);

            if (!success)
            {
                return StatusCode(500, new { success = false, message = "Failed to update service (unexpected error)." });
            }

            return Ok(new { success = true, message = "Service updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while updating the service.", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        try
        {
            var serviceToDelete = await _serviceService.GetServiceByIdAsync(id);
            if (serviceToDelete == null)
            {
                return NotFound(new { success = false, message = "Service not found." });
            }

            var business = await _businessService.GetBusinessByIdAsync(serviceToDelete.business_id);
            if (business == null || business.owner_user_id != userId)
            {
                return Forbid("You do not have permission to delete this service.");
            }

            var success = await _serviceService.DeleteServiceAsync(id);

            if (!success)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete service (unexpected error or service not found)." });
            }

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            string errorMessage = "An error occurred while deleting the service.";
            if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_"))
            {
                errorMessage = "Cannot delete the service as it is linked to other entities (e.g., service providers, appointments). Delete dependent records first.";
            }
            return StatusCode(409, new { success = false, message = errorMessage, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while deleting the service.", error = ex.Message });
        }
    }
}