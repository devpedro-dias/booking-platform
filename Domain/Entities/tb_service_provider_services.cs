using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class tb_service_provider_services
{
    public Guid service_provider_id { get; set; }
    public Guid service_id { get; set; }
    public virtual tb_service_provider service_provider { get; set; }
    public virtual tb_service service { get; set; }
}
