using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("sro")]
public class Sro
{
    [Key]
    [Column("user_id")]
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    // relationships
    public virtual User User { get; set; }
}