using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityConfigurations;

public class ServiceConfig : IEntityTypeConfiguration<tb_service>
{
    public void Configure(EntityTypeBuilder<tb_service> builder)
    {
        builder.HasKey(s => s.id);
        builder.Property(s => s.id).ValueGeneratedOnAdd();

        builder.Property(s => s.name)
            .IsRequired()
            .HasColumnType("varchar(100)");

        builder.Property(s => s.description)
            .IsRequired(false)
            .HasColumnType("varchar(500)");

        builder.Property(s => s.duration_in_minutes)
            .IsRequired();

        builder.Property(s => s.price_in_cents)
            .HasColumnType("decimal(38,2)")
            .IsRequired();

        // Relacionamento 1:N com tb_business (já configurado no BusinessConfig)
        //builder.HasOne(s => s.business)
        //    .WithMany(b => b.Services)
        //    .HasForeignKey(s => s.business_id)
        //    .OnDelete(DeleteBehavior.Cascade); // Ou Restrict, se necessário

        builder.HasMany(s => s.ServiceProviderServices)
            .WithOne(sps => sps.service)
            .HasForeignKey(sps => sps.service_id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}