using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("class_days")]
public class ClassDays
{
    public ClassDays()
    {
        Classes = new HashSet<Class>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("value")]
    [StringLength(50)]
    public string Value { get; set; }

    // relationships
    public virtual ICollection<Class> Classes { get; set; }
}