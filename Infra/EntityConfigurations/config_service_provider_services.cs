using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityConfigurations;

public class ServiceProviderServicesConfig : IEntityTypeConfiguration<tb_service_provider_services>
{
    public void Configure(EntityTypeBuilder<tb_service_provider_services> builder)
    {
        builder.HasKey(sps => new { sps.service_provider_id, sps.service_id });

        builder.HasOne(sps => sps.service_provider)
            .WithMany(sp => sp.ServiceProviderServices)
            .HasForeignKey(sps => sps.service_provider_id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sps => sps.service)
            .WithMany(s => s.ServiceProviderServices)
            .HasForeignKey(sps => sps.service_id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}