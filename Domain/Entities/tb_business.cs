using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class tb_business
{
    public Guid id { get; set; }
    public string name { get; set; }
    public string address { get; set; }
    public string phone_number { get; set; }
    public string owner_user_id { get; set; }
    
    public virtual ApplicationUser owner_user { get; set; }

    public ICollection<tb_service_provider> ServiceProviders { get; set; }
    public ICollection<tb_service> Services { get; set; }
    public ICollection<tb_appointments> Appointments { get; set; }
}
