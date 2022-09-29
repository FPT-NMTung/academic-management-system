using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("district")]
public class District
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("province_id")]
    public int ProvinceId { get; set; }
    public Province Province { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("prefix")]
    public string Prefix { get; set; }
    
    public ICollection<Ward> Wards { get; set; }
}