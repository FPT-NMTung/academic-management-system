using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("room")]
public class Room
{
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
    
    // relationships
    public virtual RoomType RoomType { get; set; }
    public virtual Center Center { get; set; }
}