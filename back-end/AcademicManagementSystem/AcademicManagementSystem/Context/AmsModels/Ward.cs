using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("ward")]
public class Ward
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("province_id")]
    public int ProvinceId { get; set; }
    
    [Column("district_id")]    
    public int DistrictId { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("prefix")]
    public string Prefix { get; set; }
    
    // relationships
    public virtual Province Province{ get; set; }
    public virtual District District { get; set; }
}