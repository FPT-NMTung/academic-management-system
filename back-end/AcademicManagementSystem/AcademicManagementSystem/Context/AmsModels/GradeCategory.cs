using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("grade_category")]
public class GradeCategory
{
    public GradeCategory()
    {
        GradeItems = new HashSet<GradeItem>();
    }

    [Key] 
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    public string Name { get; set; }
    
    public virtual ICollection<GradeItem> GradeItems { get; set; }
}