using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("gpa_record")]
public class GpaRecord
{
    public GpaRecord()
    {
        GpaRecordsAnswers = new HashSet<GpaRecordAnswer>();
    }
    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("class_id")]
    public int ClassId { get; set; }
    
    [Column("teacher_id")]
    public int TeacherId { get; set; }
    
    [Column("module_id")]
    public int ModuleId { get; set; }
    
    [Column("session_id")]
    public int SessionId { get; set; }
    
    [Column("student_id")]
    public int StudentId { get; set; }
    
    [Column("form_id")]
    public int FormId { get; set; }
    
    [Column("comment")]
    [StringLength(1000)]
    public string? Comment { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // relationships
    public virtual Class Class { get; set; }
    public virtual Teacher Teacher { get; set; }
    public virtual Module Module { get; set; }
    public virtual Session Session { get; set; }
    public virtual Student Student { get; set; }
    public virtual Form Form { get; set; }
    public virtual ICollection<GpaRecordAnswer> GpaRecordsAnswers { get; set; }
}