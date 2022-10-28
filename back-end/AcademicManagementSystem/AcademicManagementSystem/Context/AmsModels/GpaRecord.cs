using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("gpa_record")]
public class GpaRecord
{
    public GpaRecord()
    {
        Answers = new HashSet<Answer>();
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
    public virtual Form Form { get; set; }
    public virtual ICollection<Answer> Answers { get; set; }
}