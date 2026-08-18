using Microsoft.EntityFrameworkCore;
using MachineShopManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MachineShopManager.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectPhoto> ProjectPhotos => Set<ProjectPhoto>();

    public DbSet<ProjectHistory> ProjectHistories => Set<ProjectHistory>();

    public DbSet<ProjectServiceRequirement> ProjectServiceRequirements => Set<ProjectServiceRequirement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProjectHistory>()
            .HasOne(h => h.Project)
            .WithMany(p => p.History)
            .HasForeignKey(h => h.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
