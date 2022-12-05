using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("answer")]
public class Answer
{
    public Answer()
    {
        GpaRecordsAnswers = new HashSet<GpaRecordAnswer>();
    }
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("question_id")]
    public int QuestionId { get; set; }
    
    [Column("answer_no")]
    public int AnswerNo { get; set; }
    
    [Column("content")]
    [StringLength(255)]
    public string Content { get; set; }
    
    // relationships
    public virtual Question Question { get; set; }
    public virtual ICollection<GpaRecordAnswer> GpaRecordsAnswers { get; set; }
}