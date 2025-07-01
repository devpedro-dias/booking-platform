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
public class ServiceProviderController : ControllerBase
{
    private readonly IServiceProviderService _serviceProviderService;
    private readonly IBusinessService _businessService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ServiceProviderController(
        IServiceProviderService serviceProviderService,
        IBusinessService businessService,
        UserManager<ApplicationUser> userManager
    )
    {
        _serviceProviderService = serviceProviderService;
        _businessService = businessService;
        _userManager = userManager;
    }

    private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    private ServiceProviderResponseDTO MapServiceProviderToDto(tb_service_provider serviceProvider)
    {
        return new ServiceProviderResponseDTO
        {
            Id = serviceProvider.id.ToString(),
            BusinessId = serviceProvider.business_id.ToString(),
            UserId = serviceProvider.user_id,
            Name = serviceProvider.name,
            AvatarImageUrl = serviceProvider.avatar_image_url,
            AvailableFromWeekday = serviceProvider.available_from_weekday,
            AvailableToWeekday = serviceProvider.available_to_weekday,
            AvailableFromTime = serviceProvider.available_from_time,
            AvailableToTime = serviceProvider.available_to_time
        };
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceProviderResponseDTO>>> Get([FromQuery] Guid businessId)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        var business = await _businessService.GetBusinessByIdAsync(businessId);
        if (business == null || business.owner_user_id != userId)
        {
            return NotFound("Business not found or you do not have permission to view its service providers.");
        }

        try
        {
            var serviceProviders = await _serviceProviderService.GetAllServiceProvidersByBusinessIdAsync(businessId);
            var responseDtos = serviceProviders.Select(MapServiceProviderToDto).ToList();
            return Ok(responseDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while retrieving service providers.", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceProviderResponseDTO>> GetById(Guid id)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        var serviceProvider = await _serviceProviderService.GetServiceProviderByIdAsync(id);

        if (serviceProvider == null)
        {
            return NotFound("Service provider not found.");
        }

        var business = await _businessService.GetBusinessByIdAsync(serviceProvider.business_id);
        if (business == null || business.owner_user_id != userId)
        {
            return Forbid("You do not have permission to view this service provider.");
        }

        return Ok(MapServiceProviderToDto(serviceProvider));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceProviderResponseDTO>> Create([FromBody] ServiceProviderRequestDTO serviceProviderDto)
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

        var business = await _businessService.GetBusinessByIdAsync(serviceProviderDto.BusinessId);
        if (business == null || business.owner_user_id != userId)
        {
            return NotFound("Business not found or you do not have permission to add service providers to it.");
        }

        if (!string.IsNullOrEmpty(serviceProviderDto.UserId))
        {
            var targetUser = await _userManager.FindByIdAsync(serviceProviderDto.UserId);
            if (targetUser == null)
            {
                return BadRequest("Provided User ID does not exist.");
            }
        }


        try
        {
            var serviceProvider = new tb_service_provider
            {
                id = Guid.NewGuid(),
                business_id = serviceProviderDto.BusinessId,
                user_id = serviceProviderDto.UserId,
                name = serviceProviderDto.Name.Trim(),
                avatar_image_url = serviceProviderDto.AvatarImageUrl?.Trim(),
                available_from_weekday = serviceProviderDto.AvailableFromWeekday,
                available_to_weekday = serviceProviderDto.AvailableToWeekday,
                available_from_time = serviceProviderDto.AvailableFromTime,
                available_to_time = serviceProviderDto.AvailableToTime
            };

            var createdServiceProvider = await _serviceProviderService.CreateServiceProviderAsync(serviceProvider);
            var responseDto = MapServiceProviderToDto(createdServiceProvider);

            return CreatedAtAction(nameof(GetById), new { id = createdServiceProvider.id }, responseDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while creating the service provider.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] ServiceProviderRequestDTO serviceProviderDto)
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
            var existingServiceProvider = await _serviceProviderService.GetServiceProviderByIdAsync(id);
            if (existingServiceProvider == null)
            {
                return NotFound(new { success = false, message = "Service provider not found." });
            }

            var business = await _businessService.GetBusinessByIdAsync(existingServiceProvider.business_id);
            if (business == null || business.owner_user_id != userId)
            {
                return Forbid("You do not have permission to update this service provider.");
            }

            if (existingServiceProvider.business_id != serviceProviderDto.BusinessId)
            {
                var newBusiness = await _businessService.GetBusinessByIdAsync(serviceProviderDto.BusinessId);
                if (newBusiness == null || newBusiness.owner_user_id != userId)
                {
                    return Forbid("You do not have permission to move this service provider to the specified business.");
                }
            }

            if (!string.IsNullOrEmpty(serviceProviderDto.UserId))
            {
                var targetUser = await _userManager.FindByIdAsync(serviceProviderDto.UserId);
                if (targetUser == null)
                {
                    return BadRequest("Provided User ID does not exist.");
                }
            }
        
            existingServiceProvider.business_id = serviceProviderDto.BusinessId;
            existingServiceProvider.user_id = serviceProviderDto.UserId;
            existingServiceProvider.name = serviceProviderDto.Name.Trim();
            existingServiceProvider.avatar_image_url = serviceProviderDto.AvatarImageUrl?.Trim();
            existingServiceProvider.available_from_weekday = serviceProviderDto.AvailableFromWeekday;
            existingServiceProvider.available_to_weekday = serviceProviderDto.AvailableToWeekday;
            existingServiceProvider.available_from_time = serviceProviderDto.AvailableFromTime;
            existingServiceProvider.available_to_time = serviceProviderDto.AvailableToTime;

            var success = await _serviceProviderService.UpdateServiceProviderAsync(existingServiceProvider);

            if (!success)
            {
                return StatusCode(500, new { success = false, message = "Failed to update service provider (unexpected error)." });
            }

            return Ok(new { success = true, message = "Service provider updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while updating the service provider.", error = ex.Message });
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
            var serviceProviderToDelete = await _serviceProviderService.GetServiceProviderByIdAsync(id);
            if (serviceProviderToDelete == null)
            {
                return NotFound(new { success = false, message = "Service provider not found." });
            }

            var business = await _businessService.GetBusinessByIdAsync(serviceProviderToDelete.business_id);
            if (business == null || business.owner_user_id != userId)
            {
                return Forbid("You do not have permission to delete this service provider.");
            }

            var success = await _serviceProviderService.DeleteServiceProviderAsync(id);

            if (!success)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete service provider (unexpected error)." });
            }

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            string errorMessage = "An error occurred while deleting the service provider.";
            if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_"))
            {
                errorMessage = "Cannot delete the service provider as it is linked to other entities (e.g., appointments, service provider services). Delete dependent records first.";
            }
            return StatusCode(409, new { success = false, message = errorMessage, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while deleting the service provider.", error = ex.Message });
        }
    }
}