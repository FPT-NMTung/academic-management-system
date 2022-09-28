using AcademicManagementSystem.Context.AmsModels;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Context;

public class AmsContext : DbContext
{
    public AmsContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.ClientNoAction;
        }
    }

    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Ward> Wards { get; set; }
}