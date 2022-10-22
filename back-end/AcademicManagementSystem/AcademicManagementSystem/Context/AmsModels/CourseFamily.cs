using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("course_family")]
public class CourseFamily
{
    //constructor
    public CourseFamily()
    {
        Courses = new HashSet<Course>();
        Classes = new HashSet<Class>();
    }
    
    [Key] [Column("code")] 
    [StringLength(100)]
    public string Code { get; set; }

    [Column("name")] 
    [StringLength(255)]
    public string Name { get; set; }

    [Column("published_year")] 
    public int PublishedYear { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")] 
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")] 
    public DateTime UpdatedAt { get; set; }
    
    // relationships
    public virtual ICollection<Course> Courses { get; set; }
    public virtual ICollection<Class> Classes { get; set; }

}