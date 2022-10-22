using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("skill")]
public class Skill
{
    public Skill()
    {
        this.TeachersSkills = new HashSet<TeacherSkill>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    
    // relationships
    public virtual ICollection<TeacherSkill> TeachersSkills { get; set; }
}