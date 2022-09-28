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
        
        modelBuilder.Entity<User>()
            .HasIndex(u => new {u.Email, u.MobilePhone, u.EmailCompany, u.CitizenIdentityCardNo})
            .IsUnique();
    }

    public DbSet<Province> Provinces { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<Ward> Wards { get; set; }
    public DbSet<Center> Centers { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ActiveRefreshToken> ActiveRefreshTokens { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Sro> Sros { get; set; }
}