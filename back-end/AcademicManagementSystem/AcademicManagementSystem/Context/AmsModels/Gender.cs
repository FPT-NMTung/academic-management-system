using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("gender")]
public class Gender
{
    public Gender()
    {
        this.Users = new HashSet<User>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("value")]
    [StringLength(255)]
    public string Value { get; set; }
    
    // relationships
    public virtual ICollection<User> Users { get; set; }
}