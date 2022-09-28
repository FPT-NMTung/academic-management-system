using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("room")]
public class Room
{
    [Key]
    [Column("room_code")]
    public string RoomCode { get; set; }
    public RoomType RoomType { get; set; }
    
    [Column("center_id")]
    public int CenterId { get; set; }
    public Center Center { get; set; }
    
    [Column("room_type_id")]
    public int RoomTypeId { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Column("capacity")]
    public int Capacity { get; set; }
}