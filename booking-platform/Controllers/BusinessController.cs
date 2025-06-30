using booking_platform.DTO;
using Domain.Entities;
using Domain.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace booking_platform.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class BusinessController : ControllerBase
{
    private readonly IBusinessService _businessService;

    public BusinessController(IBusinessService businessService)
    {
        _businessService = businessService;
    }

    private string CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    [HttpGet]
    public async Task<ActionResult<List<BusinessResponseDTO>>> Get()
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        try
        {
            var businesses = await _businessService.GetAllBusinessesByUserIdAsync(userId);

            var responseDtos = businesses.Select(b => new BusinessResponseDTO
            {
                Id = b.id.ToString(),
                Name = b.name,
                Address = b.address,
                PhoneNumber = b.phone_number,
                OwnerUserId = b.owner_user_id,
                Services = (b.Services ?? new List<tb_service>())
                           .Select(s => new ServiceResponseDTO
                           {
                    Id = s.id.ToString(),
                    Name = s.name,
                    Description = s.description,
                    DurationInMinutes = s.duration_in_minutes,
                    PriceInCents = s.price_in_cents
                }).ToList(),
                ServiceProviders = (b.ServiceProviders ?? new List<tb_service_provider>())
                                   .Select(sp => new ServiceProviderDTO
                                   {
                                       Id = sp.id.ToString(),
                                       Name = sp.name,
                                       AvatarImageUrl = sp.avatar_image_url
                                   }).ToList()
            }).ToList();

            return Ok(responseDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while retrieving businesses.", error = ex.Message });
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<BusinessResponseDTO>> GetById(Guid id)
    {
        var userId = CurrentUserId;
        if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found or user not authenticated.");
        }

        var business = await _businessService.GetBusinessByIdAsync(id);

        if (business == null)
        {
            return NotFound();
        }

        if (business.owner_user_id != userId)
        {
            return Forbid();
        }

        var responseDto = new BusinessResponseDTO
        {
            Id = business.id.ToString(),
            Name = business.name,
            Address = business.address,
            PhoneNumber = business.phone_number,
            OwnerUserId = business.owner_user_id,
            Services = (business.Services ?? new List<tb_service>())
                       .Select(s => new ServiceResponseDTO
                       {
                           Id = s.id.ToString(),
                           Name = s.name,
                           Description = s.description,
                           DurationInMinutes = s.duration_in_minutes,
                           PriceInCents = s.price_in_cents
                       }).ToList(),
            ServiceProviders = (business.ServiceProviders ?? new List<tb_service_provider>())
                               .Select(sp => new ServiceProviderDTO
                               {
                                   Id = sp.id.ToString(),
                                   Name = sp.name,
                                   AvatarImageUrl = sp.avatar_image_url
                               }).ToList()
        };

        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<ActionResult<BusinessResponseDTO>> Create([FromBody] BusinessRequestDTO businessDto)
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
            var business = new tb_business
            {
                name = businessDto.Name?.Trim(),
                address = businessDto.Address?.Trim(),
                phone_number = businessDto.PhoneNumber?.Trim(),
                owner_user_id = userId
            };

            var businessExists = await _businessService.BusinessNameExistsForUserAsync(business.name, userId);
            if (businessExists)
            {
                return Conflict(new { success = false, message = "A business with the same name already exists for this user." });
            }

            var createdBusiness = await _businessService.CreateBusinessAsync(business);

            var responseDto = new BusinessResponseDTO
            {
                Id = createdBusiness.id.ToString(),
                Name = createdBusiness.name,
                Address = createdBusiness.address,
                PhoneNumber = createdBusiness.phone_number,
                OwnerUserId = createdBusiness.owner_user_id,
                Services = new List<ServiceResponseDTO>(),
                ServiceProviders = new List<ServiceProviderDTO>()
            };

            return CreatedAtAction(nameof(GetById), new { id = createdBusiness.id }, responseDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while creating the business.", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] BusinessRequestDTO businessDto)
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
            var existingBusiness = await _businessService.GetBusinessByIdAsync(id);
            if (existingBusiness == null || existingBusiness.owner_user_id != userId)
            {
                return NotFound(new { success = false, message = "Business not found or you do not have permission to edit it." });
            }

            var businessNameExists = await _businessService.BusinessNameExistsForUserAsync(businessDto.Name, userId, id);
            if (businessNameExists)
            {
                return Conflict(new { success = false, message = "Another business with the same name already exists for this user." });
            }

            existingBusiness.name = businessDto.Name?.Trim();
            existingBusiness.address = businessDto.Address?.Trim();
            existingBusiness.phone_number = businessDto.PhoneNumber?.Trim();

            var success = await _businessService.UpdateBusinessAsync(existingBusiness);

            if (!success)
            {
                return StatusCode(500, new { success = false, message = "Failed to update business (unexpected error)." });
            }

            return Ok(new { success = true, message = "Business updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while updating the business.", error = ex.Message });
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
            var businessToDelete = await _businessService.GetBusinessByIdAsync(id);
            if (businessToDelete == null || businessToDelete.owner_user_id != userId)
            {
                return NotFound(new { success = false, message = "Business not found or you do not have permission to delete it." });
            }

            var success = await _businessService.DeleteBusinessAsync(id);

            if (!success)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete business (unexpected error)." });
            }

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            string errorMessage = "An error occurred while deleting the business.";
            if (ex.InnerException != null && ex.InnerException.Message.Contains("FK_"))
            {
                errorMessage = "Cannot delete the business as it is used in other entities (services, service providers, appointments).";
            }
            return StatusCode(409, new { success = false, message = errorMessage, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "An error occurred while deleting the business.", error = ex.Message });
        }
    }
}