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

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<ProjectPhoto> ProjectPhotos => Set<ProjectPhoto>();
}