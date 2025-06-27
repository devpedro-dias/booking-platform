using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityConfigurations;

public class AppointmentConfig : IEntityTypeConfiguration<tb_appointments>
{
    public void Configure(EntityTypeBuilder<tb_appointments> builder)
    {
        builder.HasKey(a => a.id);
        builder.Property(a => a.id).ValueGeneratedOnAdd();

        builder.Property(a => a.start_date_time)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(a => a.end_date_time)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(a => a.total_price_in_cents)
            .IsRequired();

        builder.Property(a => a.status)
            .IsRequired()
            .HasColumnType("varchar(50)");

        builder.Property(a => a.created_at)
            .IsRequired();


        builder.HasOne(a => a.business)
            .WithMany(b => b.Appointments)
            .HasForeignKey(a => a.business_id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.customer)
            .WithMany(u => u.customer_appointments)
            .HasForeignKey(a => a.customer_id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.service_provider)
            .WithMany(sp => sp.Appointments)
            .HasForeignKey(a => a.service_provider_id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.service)
            .WithMany(s => s.Appointments)
            .HasForeignKey(a => a.service_id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}