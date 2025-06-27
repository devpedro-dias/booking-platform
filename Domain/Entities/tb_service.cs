using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class tb_service
{
    public Guid id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public int price_in_cents { get; set; }
    public int duration_in_minutes { get; set; }
    public Guid business_id { get; set; }
    public virtual tb_business business { get; set; }

    public ICollection<tb_service_provider_services> ServiceProviderServices { get; set; }
    public virtual ICollection<tb_appointments> Appointments { get; set; } = new List<tb_appointments>();
}
