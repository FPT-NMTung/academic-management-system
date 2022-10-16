using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("grade_category")]
public class GradeCategory
{
    public GradeCategory()
    {
        GradeCategoryModules = new HashSet<GradeCategoryModule>();
    }

    [Key] 
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; }
    
    public virtual ICollection<GradeCategoryModule> GradeCategoryModules { get; set; }
}