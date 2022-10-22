using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("teacher_skill")]
public class TeacherSkill
{
    [Column("teacher_id")]
    [ForeignKey("Teacher")]
    public int TeacherId { get; set; }
    
    [Column("skill_id")]
    public int SkillId { get; set; }
    
    // relationships
    public virtual Teacher Teacher { get; set; }
    public virtual Skill Skill { get; set; }
}