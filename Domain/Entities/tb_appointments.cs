using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class tb_appointments
{
    public Guid id { get; set; }
    public DateTime start_date_time { get; set; }
    public DateTime end_date_time { get; set; }
    public int total_price_in_cents { get; set; }
    public string status { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;

    public Guid business_id { get; set; }
    public virtual tb_business business { get; set; }

    public string customer_id { get; set; }
    public virtual ApplicationUser customer { get; set; }

    public Guid service_provider_id { get; set; }
    public virtual tb_service_provider service_provider { get; set; }

    public Guid service_id { get; set; }
    public virtual tb_service service { get; set; }
}
