using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("class_status")]
public class ClassStatus
{
    public ClassStatus()
    {
        Classes = new HashSet<Class>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("value")]
    [StringLength(50)]
    public string Value { get; set; }
    
    public ICollection<Class> Classes { get; set; }
}