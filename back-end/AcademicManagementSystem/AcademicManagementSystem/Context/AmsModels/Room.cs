using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("room")]
public class Room
{
    public Room()
    {
        ClassSchedulesTheoryRoom = new HashSet<ClassSchedule>();
        ClassSchedulesLabRoom = new HashSet<ClassSchedule>();
        ClassSchedulesExamRoom = new HashSet<ClassSchedule>();
        Sessions = new HashSet<Session>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("center_id")]
    public int CenterId { get; set; }
    
    [Column("room_type_id")]
    public int RoomTypeId { get; set; }

    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    
    [Column("capacity")]
    public int Capacity { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    // relationships
    public virtual RoomType RoomType { get; set; }
    public virtual Center Center { get; set; }
    [InverseProperty("TheoryRoom")]
    public virtual ICollection<ClassSchedule> ClassSchedulesTheoryRoom { get; set; }
    [InverseProperty("LabRoom")]
    public virtual ICollection<ClassSchedule> ClassSchedulesLabRoom { get; set; }
    [InverseProperty("ExamRoom")]
    public virtual ICollection<ClassSchedule> ClassSchedulesExamRoom { get; set; }
    public virtual ICollection<Session> Sessions { get; set; } 
}