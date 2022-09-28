using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("role")]
public class Role
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("value")]
    public string Value { get; set; }
    
}