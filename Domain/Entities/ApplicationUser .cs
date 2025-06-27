using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    public ICollection<tb_appointments> customer_appointments { get; set; }

    public virtual tb_service_provider service_provider_profile { get; set; }
}
