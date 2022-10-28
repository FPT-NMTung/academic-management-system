using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("attendance")]
public class Attendance
{
    [Column("session_id")]
    public int SessionId { get; set; }
    
    [Column("student_id")]
    public int StudentId { get; set; }
    
    [Column("attendance_status_id")]
    public int AttendanceStatusId { get; set; }
    
    [Column("note")]
    [StringLength(255)]
    public string? Note { get; set; }

    // relationships
    public virtual AttendanceStatus AttendanceStatus { get; set; }
    public virtual Session Session { get; set; }
    public virtual Student Student { get; set; }
}