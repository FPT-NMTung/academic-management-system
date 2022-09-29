using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("district")]
public class District
{
    public District()
    {
        this.Wards = new HashSet<Ward>();
        this.Users = new HashSet<User>();
        this.Centers = new HashSet<Center>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("province_id")]
    public int ProvinceId { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("prefix")]
    public string Prefix { get; set; }
    
    // relationships
    public virtual Province Province { get; set; }
    public virtual ICollection<Ward> Wards { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Center> Centers { get; set; }
}