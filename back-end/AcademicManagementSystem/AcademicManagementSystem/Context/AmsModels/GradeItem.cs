using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("grade_item")]
public class GradeItem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("module_id")]
    public int ModuleId { get; set; }
    
    [Column("grade_category_id")]
    public int GradeCategoryId { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    [Column("weight")]
    public int Weight { get; set; }
    
    // relationships
    public virtual Module Module { get; set; }
    public virtual GradeCategory GradeCategory { get; set; }
}