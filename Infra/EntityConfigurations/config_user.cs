using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityConfigurations;

public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {

        builder.Property(u => u.Name)
            .IsRequired()
            .HasColumnType("varchar(150)");

        builder.HasOne(u => u.service_provider_profile)
            .WithOne(sp => sp.user)
            .HasForeignKey<tb_service_provider>(sp => sp.user_id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.customer_appointments)
            .WithOne(a => a.customer)
            .HasForeignKey(a => a.customer_id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}