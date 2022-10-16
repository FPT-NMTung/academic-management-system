using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("room_type")]
public class RoomType
{
    public RoomType()
    {
        this.Rooms = new HashSet<Room>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("value")]
    [StringLength(50)]
    public string Value { get; set; }
    
    // relationships
    public virtual ICollection<Room> Rooms { get; set; }
}