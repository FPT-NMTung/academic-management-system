using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("grade_category_module")]
public class GradeCategoryModule
{
    public GradeCategoryModule()
    {
        GradeItems = new HashSet<GradeItem>();
    }
    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("module_id")]
    public int ModuleId { get; set; }
    
    [Column("grade_category_id")]
    public int GradeCategoryId { get; set; }
    
    [Column("total_weight")]
    public int TotalWeight { get; set; }
    
    [Column("quantity_grade_item")]
    public int QuantityGradeItem { get; set; }
    
    // relationships
    public virtual Module Module { get; set; }
    public virtual GradeCategory GradeCategory { get; set; }
    public virtual ICollection<GradeItem> GradeItems { get; set; }
}