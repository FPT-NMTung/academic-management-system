using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("room_type")]
public class RoomType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("value")]
    [StringLength(100)]
    public string Value { get; set; }
    
    public ICollection<Room> Rooms { get; set; }
}