using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("center")]
public class Center
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("province_id")]
    public int ProvinceId { get; set; }
    public Province Province { get; set; }
    
    [Column("district_id")]
    public int DistrictId { get; set; }
    public District District { get; set; }
    
    [Column("ward_id")]
    public int WardId { get; set; }
    public Ward Ward { get; set; }
    
    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
}