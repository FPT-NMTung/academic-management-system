using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;
[Table("province")]
public class Province
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("code")]
    public string Code { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    public ICollection<District> Districts { get; set; }
    public ICollection<Ward> Wards { get; set; }
}