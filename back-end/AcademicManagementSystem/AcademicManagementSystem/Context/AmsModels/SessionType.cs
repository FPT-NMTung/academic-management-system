using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("session_type")]
public class SessionType
{
    public SessionType()
    {
        Sessions = new HashSet<Session>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("value")]
    [StringLength(50)]
    public string Value { get; set; }
    
    // relationships
    public virtual ICollection<Session> Sessions { get; set; }
}