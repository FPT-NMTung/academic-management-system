using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("course")]
public class Course
{
    // constructor
    public Course()
    {
        CoursesModulesSemesters = new HashSet<CourseModuleSemester>();
        Students = new HashSet<Student>();
    }
    
    [Key]
    [Column("code")]
    [StringLength(100)]
    public string Code { get; set; }
    
    [Column("course_family_code")]
    [StringLength(100)]
    public string CourseFamilyCode { get; set; }
    
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    
    [Column("semester_count")]
    public int SemesterCount { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // relationship
    public virtual CourseFamily CourseFamily { get; set; }
    public virtual ICollection<CourseModuleSemester> CoursesModulesSemesters { get; set; }
    public virtual ICollection<Student> Students { get; set; }

}