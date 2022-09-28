using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("sro")]
public class Sro
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }
    public User User { get; set; }
}