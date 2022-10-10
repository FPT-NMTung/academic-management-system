using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("class")]
public class Class
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("center_id")]
    public int CenterId { get; set; }
    
    [Column("course_code")]
    public string CourseCode { get; set; }
    
    [Column("class_days_id")]   
    public int ClassDaysId { get; set; }
    
    [Column("class_status_id")]
    public int ClassStatusId { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("start_date")]
    public DateTime StartDate { get; set; }
    
    [Column("completion_date")]
    public DateTime CompletionDate { get; set; }
    
    [Column("graduation_date")]
    public DateTime GraduationDate { get; set; }
    
    [Column("class_hour_start", TypeName = "time")]
    public TimeSpan ClassHourStart { get; set; }
    
    [Column("class_hour_end", TypeName = "time")]
    public TimeSpan ClassHourEnd { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // relationships
    public virtual Center Center { get; set; }
    public virtual Course Course { get; set; }
    public virtual ClassDays ClassDays { get; set; }
    public virtual ClassStatus ClassStatus { get; set; }
}