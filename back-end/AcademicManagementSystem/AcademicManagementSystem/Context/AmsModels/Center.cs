using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("center")]
public class Center
{
    public Center()
    {
        Rooms = new HashSet<Room>();
        Users = new HashSet<User>();
        Modules = new HashSet<Module>();
        Classes = new HashSet<Class>();
        DayOffs = new HashSet<DayOff>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("province_id")]
    public int ProvinceId { get; set; }
    
    [Column("district_id")]
    public int DistrictId { get; set; }
    
    [Column("ward_id")]
    public int WardId { get; set; }
    
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // relationships
    public virtual Ward Ward { get; set; }
    public virtual District District { get; set; }
    public virtual Province Province { get; set; }
    public virtual ICollection<Room> Rooms { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Module> Modules { get; set; }
    public virtual ICollection<Class> Classes { get; set; }
    public virtual ICollection<DayOff> DayOffs { get; set; }
}