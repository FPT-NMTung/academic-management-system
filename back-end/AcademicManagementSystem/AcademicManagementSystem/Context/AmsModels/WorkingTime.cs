using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("working_time")]
public class WorkingTime
{
    public WorkingTime()
    {
        Teachers = new HashSet<Teacher>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("value")]
    [StringLength(50)]
    public string Value { get; set; }
    
    // relationships
    public virtual ICollection<Teacher> Teachers { get; set; }

}