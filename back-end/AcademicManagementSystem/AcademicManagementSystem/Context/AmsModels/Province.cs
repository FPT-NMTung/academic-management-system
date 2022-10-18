using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;
[Table("province")]
public class Province
{
    public Province()
    {
        this.Districts = new HashSet<District>();
        this.Wards = new HashSet<Ward>();
        this.Users = new HashSet<User>();
        this.Centers = new HashSet<Center>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("code")]
    public string Code { get; set; }
    
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    
    // relationships
    public virtual ICollection<District> Districts { get; set; }
    public virtual ICollection<Ward> Wards { get; set; }
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<Center> Centers { get; set; }
}