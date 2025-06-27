using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class tb_service_provider
{
    public Guid id { get; set; }

    public Guid business_id { get; set; }
    public virtual tb_business business { get; set; }

    public string user_id { get; set; }
    public virtual ApplicationUser user { get; set; }

    public string name { get; set; }
    public string avatar_image_url { get; set; }

    public int available_from_weekday { get; set; }
    public int available_to_weekday { get; set; }
    public TimeSpan available_from_time { get; set; }
    public TimeSpan available_to_time { get; set; }

    public ICollection<tb_appointments> Appointments { get; set; }
    public ICollection<tb_service_provider_services> ServiceProviderServices { get; set; }
}
