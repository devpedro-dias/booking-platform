using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public ICollection<tb_appointments> customer_appointments { get; set; }

    public virtual tb_service_provider service_provider_profile { get; set; }
}
