using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityConfigurations;

public class BusinessConfig : IEntityTypeConfiguration<tb_business>
{
    public void Configure(EntityTypeBuilder<tb_business> builder)
    {
        builder.HasKey(b => b.id);
        builder.Property(b => b.id).ValueGeneratedOnAdd();

        builder.Property(b => b.name)
            .IsRequired()
            .HasColumnType("varchar(255)");

        builder.Property(b => b.address)
            .IsRequired()
            .HasColumnType("varchar(255)");

        builder.Property(b => b.phone_number)
            .IsRequired()
            .HasColumnType("varchar(20)");

        builder.HasOne(b => b.owner_user)
           .WithMany()
           .HasForeignKey(b => b.owner_user_id)
           .OnDelete(DeleteBehavior.Restrict)

           .IsRequired();

        builder.HasMany(b => b.Services)
            .WithOne(s => s.business)
            .HasForeignKey(s => s.business_id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.ServiceProviders)
            .WithOne(sp => sp.business)
            .HasForeignKey(sp => sp.business_id)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Appointments)
            .WithOne(a => a.business)
            .HasForeignKey(a => a.business_id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}