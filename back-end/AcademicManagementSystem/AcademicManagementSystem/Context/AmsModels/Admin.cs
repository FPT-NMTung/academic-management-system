using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("admin")]
public class Admin
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }
    public User User { get; set; }
}