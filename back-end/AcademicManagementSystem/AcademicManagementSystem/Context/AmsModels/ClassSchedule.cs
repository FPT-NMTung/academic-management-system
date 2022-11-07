using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("class_schedule")]
public class ClassSchedule
{
    public ClassSchedule()
    {
        Sessions = new HashSet<Session>();
    }
    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("class_id")]
    public int ClassId { get; set; }
    
    [Column("module_id")]
    public int ModuleId { get; set; }
    
    [Column("teacher_id")]
    public int TeacherId { get; set; }
    
    [Column("class_days_id")]
    public int ClassDaysId { get; set; }
    
    [Column("working_time_id",Order = 5)]
    public int WorkingTimeId { get; set; }
    
    [Column("class_status_id")]
    public int ClassStatusId { get; set; }
    
    [ForeignKey("TheoryRoom")]
    [Column("theory_room_id")]
    public int? TheoryRoomId { get; set; }
    
    [ForeignKey("LabRoom")]
    [Column("lab_room_id")]
    public int? LabRoomId { get; set; }
    
    [ForeignKey("ExamRoom")]
    [Column("exam_room_id")]
    public int? ExamRoomId { get; set; }
    
    //days
    [Column("duration")]
    public int Duration { get; set; }
    
    [Column("start_date", TypeName = "date")]
    public DateTime StartDate { get; set; }
    
    [Column("end_date", TypeName = "date")]
    public DateTime EndDate { get; set; }
    
    [Column("theory_exam_date", TypeName = "date")]
    public DateTime? TheoryExamDate { get; set; }
    
    [Column("practical_exam_date", TypeName = "date")]
    public DateTime? PracticalExamDate { get; set; }
    
    [Column("class_hour_start", TypeName = "time")]
    public TimeSpan ClassHourStart { get; set; }
    
    [Column("class_hour_end", TypeName = "time")]
    public TimeSpan ClassHourEnd { get; set; }
    
    [Column("note")]
    [StringLength(255)]
    public string? Note { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // Relationships
    public virtual Class Class { get; set; }
    public virtual Module Module { get; set; }
    public virtual Teacher Teacher { get; set; }
    public virtual ClassDays ClassDays { get; set; }
    public virtual ClassStatus ClassStatus { get; set; }
    public virtual Room? TheoryRoom { get; set; }
    public virtual Room? LabRoom { get; set; }
    public virtual Room? ExamRoom { get; set; }
    public virtual ICollection<Session> Sessions { get; set; }
}