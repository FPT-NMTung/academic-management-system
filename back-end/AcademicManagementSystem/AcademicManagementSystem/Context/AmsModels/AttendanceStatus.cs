using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("attendance_status")]
public class AttendanceStatus
{
    public AttendanceStatus()
    {
        Attendances = new HashSet<Attendance>();
    }
    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("value")]
    [StringLength(50)]
    public string Value { get; set; }
    
    // relationships
    public virtual ICollection<Attendance> Attendances { get; set; }
}