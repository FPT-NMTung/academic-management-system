using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("student_grade")]
public class StudentGrade
{
    [Column("class_id")]
    public int ClassId { get; set; }
    
    [Column("student_id")]
    public int StudentId { get; set; }
    
    [Column("grade_item_id")]
    public int GradeItemId { get; set; }
    
    [Column("grade")]
    public double? Grade { get; set; }
    
    [Column("comment")]
    [StringLength(1000)]
    public string? Comment { get; set; }
    
    // relationships
    public virtual Class Class { get; set; }
    public virtual Student Student { get; set; }
    public virtual GradeItem GradeItem { get; set; }
}