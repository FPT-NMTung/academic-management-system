using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("session")]
public class Session
{

    public Session()
    {
        Attendances = new HashSet<Attendance>();
        GpaRecords = new HashSet<GpaRecord>();
    }
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("class_schedule_id")]
    public int ClassScheduleId { get; set; }
    
    [Column("session_type_id")]
    public int SessionTypeId { get; set; }
    
    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; }
    
    [Column("room_id", Order = 4)]
    public int RoomId { get; set; }
    
    [Column("learning_date", TypeName = "date")]
    public DateTime LearningDate { get; set; }
    
    [Column("start_time")]
    public TimeSpan StartTime { get; set; }
    
    [Column("end_time")]
    public TimeSpan EndTime { get; set; }
    
    // relationships
    public virtual ClassSchedule ClassSchedule { get; set; }
    public virtual SessionType SessionType { get; set; }
    public virtual ICollection<Attendance> Attendances { get; set; }
    public virtual ICollection<GpaRecord> GpaRecords { get; set; }
    public virtual Room Room { get; set; }

}