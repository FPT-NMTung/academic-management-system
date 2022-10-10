using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("course_module_semester")]
public class CourseModuleSemester
{
    [Column("course_code")]
    [StringLength(100)]
    public string CourseCode { get; set; } = null!;

    [Column("module_id")]
    public int ModuleId { get; set; }
    
    [Column("semester_id")]
    public int SemesterId { get; set; }
    
    // relationships
    public virtual Course Course { get; set; }
    public virtual Module Module { get; set; }
    public virtual Semester Semester { get; set; }
}