using Microsoft.EntityFrameworkCore;
using MachineShopManager.Models;

namespace MachineShopManager.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
}