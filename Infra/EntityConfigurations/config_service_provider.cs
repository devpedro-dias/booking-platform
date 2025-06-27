using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EntityConfigurations;

public class ServiceProviderConfig : IEntityTypeConfiguration<tb_service_provider>
{
    public void Configure(EntityTypeBuilder<tb_service_provider> builder)
    {
        builder.HasKey(sp => sp.id);
        builder.Property(sp => sp.id).ValueGeneratedOnAdd();

        builder.Property(sp => sp.name)
            .IsRequired()
            .HasColumnType("varchar(150)");

        builder.Property(sp => sp.avatar_image_url)
            .IsRequired(false)
            .HasColumnType("varchar(500)");

        // Relacionamento 1:1 com ApplicationUser (já configurado no ApplicationUserConfig)
        //builder.HasOne(sp => sp.user)
        //    .WithOne(u => u.service_provider_profile)
        //    .HasForeignKey<tb_service_provider>(sp => sp.user_id)
        //    .OnDelete(DeleteBehavior.Cascade); // Ou Restrict

        // Relacionamento 1:N com tb_business (já configurado no BusinessConfig)
        //builder.HasOne(sp => sp.business)
        //    .WithMany(b => b.ServiceProviders)
        //    .HasForeignKey(sp => sp.business_id)
        //    .OnDelete(DeleteBehavior.Cascade); // Ou Restrict

        // Relacionamento 1:N com tb_appointments
        builder.HasMany(sp => sp.Appointments)
            .WithOne(a => a.service_provider)
            .HasForeignKey(a => a.service_provider_id)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacionamento Muitos-para-Muitos com tb_service via tb_service_provider_services
        // A configuração para isso é principalmente na tabela de junção (ServiceProviderServicesConfig)
        builder.HasMany(sp => sp.ServiceProviderServices)
            .WithOne(sps => sps.service_provider)
            .HasForeignKey(sps => sps.service_provider_id)
            .OnDelete(DeleteBehavior.Cascade); // Se um prestador for deletado, suas associações são deletadas
    }
}