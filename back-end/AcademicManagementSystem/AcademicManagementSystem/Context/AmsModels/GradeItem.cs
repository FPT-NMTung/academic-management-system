using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("grade_item")]
public class GradeItem
{

    public GradeItem()
    {
        StudentGrades = new HashSet<StudentGrade>();
    }
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("grade_category_module_id")]
    public int GradeCategoryModuleId { get; set; }
    
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }

    // relationships
    public virtual GradeCategoryModule GradeCategoryModule { get; set; }
    public virtual ICollection<StudentGrade> StudentGrades { get; set; }
}