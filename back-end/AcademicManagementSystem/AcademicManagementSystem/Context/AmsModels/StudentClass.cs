using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("student_class")]
public class StudentClass
{
    [Column("student_id")]
    [ForeignKey("Student")]
    public int StudentId { get; set; }
    
    [Column("class_id")]
    public int ClassId { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // relationship
    public virtual Student Student { get; set; }
    public virtual Class Class { get; set; }
}