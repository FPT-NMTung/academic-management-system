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

        // Indexes User model
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.MobilePhone)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.EmailOrganization)
            .IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.CitizenIdentityCardNo)
            .IsUnique();
        modelBuilder.Entity<Teacher>()
            .HasIndex(t => t.TaxCode)
            .IsUnique();
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.EnrollNumber)
            .IsUnique();

        modelBuilder.Entity<CourseModuleSemester>()
            .HasKey(cms => new { cms.CourseCode, cms.ModuleId, cms.SemesterId });
        modelBuilder.Entity<StudentClass>()
            .HasKey(sc => new { sc.StudentId, sc.ClassId });
        modelBuilder.Entity<TeacherSkill>()
            .HasKey(ts => new { ts.TeacherId, ts.SkillId });
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
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<CourseFamily> CourseFamilies { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Semester> Semesters { get; set; }
    public DbSet<CourseModuleSemester> CoursesModulesSemesters { get; set; }
    public DbSet<TeacherType> TeacherTypes { get; set; }
    public DbSet<WorkingTime> WorkingTimes { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<ClassDays> ClassDays { get; set; }
    public DbSet<ClassStatus> ClassStatuses { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<GradeCategory> GradeCategories { get; set; }
    public DbSet<GradeCategoryModule> GradeCategoryModules { get; set; }
    public DbSet<GradeItem> GradeItems { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentClass> StudentsClasses { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<TeacherSkill> TeachersSkills { get; set; }
}