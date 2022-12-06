using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("gpa_record_answer")]
public class GpaRecordAnswer
{
    [Column("gpa_record_id")]
    public int GpaRecordId { get; set; }
    
    [Column("answer_id")]
    public int AnswerId { get; set; }
    
    // relationships
    public virtual GpaRecord GpaRecord { get; set; }
    public virtual Answer Answer { get; set; }
}