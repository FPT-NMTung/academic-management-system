using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("semester")]
public class Semester
{
    // constructor
    public Semester()
    {
        this.CoursesModuleSemesters = new HashSet<CourseModuleSemester>();
    }
    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    
    // relationships
    public virtual ICollection<CourseModuleSemester> CoursesModuleSemesters { get; set; }
}