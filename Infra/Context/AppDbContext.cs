using Domain.Entities;
using Infra.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<tb_business> Businesses { get; set; }
    public DbSet<tb_service> Services { get; set; }
    public DbSet<tb_service_provider> ServiceProviders { get; set; }
    public DbSet<tb_appointments> Appointments { get; set; }
    public DbSet<tb_service_provider_services> ServiceProviderServices { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserConfig());
        builder.ApplyConfiguration(new BusinessConfig());
        builder.ApplyConfiguration(new ServiceConfig());
        builder.ApplyConfiguration(new ServiceProviderConfig());
        builder.ApplyConfiguration(new AppointmentConfig());
        builder.ApplyConfiguration(new ServiceProviderServicesConfig());
    }
}
