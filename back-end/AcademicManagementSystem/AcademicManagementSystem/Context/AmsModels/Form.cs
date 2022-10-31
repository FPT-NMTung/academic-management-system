using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("form")]
public class Form
{
    public Form()
    {
        GpaRecords = new HashSet<GpaRecord>();
        Questions = new HashSet<Question>();
    }
    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("title")]
    [StringLength(255)]
    public string Title { get; set; }
    
    [Column("description")]
    [StringLength(1000)]
    public string Description { get; set; }
    
    // relationships
    public virtual ICollection<GpaRecord> GpaRecords { get; set; } 
    public virtual ICollection<Question> Questions { get; set; }

}