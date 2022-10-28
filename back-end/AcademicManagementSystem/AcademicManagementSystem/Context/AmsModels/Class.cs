using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("class")]
public class Class
{
    public Class()
    {
        StudentsClasses = new HashSet<StudentClass>();
        ClassSchedules = new HashSet<ClassSchedule>();
        StudentGrades = new HashSet<StudentGrade>();
        GpaRecords = new HashSet<GpaRecord>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("center_id")]
    public int CenterId { get; set; }
    
    [Column("course_family_code")]
    [StringLength(100)]
    public string CourseFamilyCode { get; set; }
    
    [Column("class_days_id")]   
    public int ClassDaysId { get; set; }
    
    [Column("class_status_id")]
    public int ClassStatusId { get; set; }
    
    [Column("sro_id")]
    [ForeignKey("Sro")]
    public int SroId { get; set; }
    
    [Column("name")]
    [StringLength(255)]
    public string Name { get; set; }
    
    [Column("start_date",TypeName = "date")]
    public DateTime StartDate { get; set; }
    
    [Column("completion_date", TypeName = "date")]
    public DateTime CompletionDate { get; set; }
    
    [Column("graduation_date", TypeName = "date")]
    public DateTime GraduationDate { get; set; }
    
    [Column("class_hour_start", TypeName = "time")]
    public TimeSpan ClassHourStart { get; set; }
    
    [Column("class_hour_end", TypeName = "time")]
    public TimeSpan ClassHourEnd { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // relationships
    public virtual Center Center { get; set; }
    public virtual CourseFamily CourseFamily { get; set; }
    public virtual ClassDays ClassDays { get; set; }
    public virtual ClassStatus ClassStatus { get; set; }
    public virtual Sro Sro { get; set; }
    public virtual ICollection<StudentClass> StudentsClasses { get; set; }
    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; }
    public virtual ICollection<StudentGrade> StudentGrades { get; set; }
    public virtual ICollection<GpaRecord> GpaRecords { get; set; } 

}