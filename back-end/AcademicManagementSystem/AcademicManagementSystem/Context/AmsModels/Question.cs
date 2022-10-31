using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("question")]
public class Question
{
    public Question()
    {
        Forms = new HashSet<Form>();
        Answers = new HashSet<Answer>();
    }
    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("content")]
    [StringLength(255)]
    public string Content { get; set; }
    
    // relationships
    public virtual ICollection<Form> Forms { get; set; }
    public virtual ICollection<Answer> Answers { get; set; }
}