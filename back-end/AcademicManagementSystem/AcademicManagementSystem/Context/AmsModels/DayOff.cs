using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("day_off")]
public class DayOff
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("teacher_id")]
    public int? TeacherId { get; set; }
    
    [Column("working_time_id", Order = 2)]
    public int WorkingTimeId { get; set; }

    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; }
    
    [Column("date", TypeName = "date")]
    public DateTime Date { get; set; }
    
    [Column("center_id")]
    public int CenterId { get; set; }
    
    // relationships
    public virtual Teacher Teacher { get; set; }
    public virtual Center Center { get; set; }
}