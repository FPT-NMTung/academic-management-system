using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("student")]
public class Student
{
    public Student()
    {
        this.StudentsClasses = new HashSet<StudentClass>();
        Attendances = new HashSet<Attendance>();
        StudentGrades = new HashSet<StudentGrade>();
        GpaRecords = new HashSet<GpaRecord>();
    }

    [Key]
    [Column("user_id")]
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [Column("enroll_number")]
    [StringLength(255)]
    public string EnrollNumber { get; set; }

    [Column("course_code")]
    [StringLength(100)]
    public string CourseCode { get; set; }
    
    [Column("status")]
    public int Status { get; set; }
    
    [Column("status_date", TypeName = "date")]
    public DateTime StatusDate { get; set; }
    
    [Column("home_phone")]
    [StringLength(15)]
    public string? HomePhone { get; set; }
    
    [Column("contact_phone")]
    [StringLength(15)]
    public string ContactPhone { get; set; }
    
    [Column("parental_name")]
    [StringLength(255)]
    public string ParentalName { get; set; }
    
    [Column("parental_relationship")]
    [StringLength(50)]
    public string ParentalRelationship { get; set; }
    
    [Column("contact_address")]
    [StringLength(255)]
    public string ContactAddress { get; set; }
    
    [Column("parental_phone")]
    [StringLength(15)]
    public string ParentalPhone { get; set; }
    
    [Column("application_date", TypeName = "date")]
    public DateTime ApplicationDate { get; set; }
    
    [Column("application_document")]
    [StringLength(255)]
    public string? ApplicationDocument { get; set; }

    [Column("high_school")]
    [StringLength(255)]
    public string? HighSchool { get; set; }
    
    [Column("university")]
    [StringLength(255)]
    public string? University { get; set; }
    
    [Column("facebook_url")]
    [StringLength(1000)]
    public string? FacebookUrl { get; set; }
    
    [Column("portfolio_url")]
    [StringLength(1000)]
    public string? PortfolioUrl { get; set; }
    
    [Column("working_company")]
    [StringLength(255)]
    public string? WorkingCompany { get; set; }
    
    [Column("company_salary")]
    public int? CompanySalary { get; set; }
    
    [Column("company_position")]
    [StringLength(255)]
    public string? CompanyPosition { get; set; }
    
    [Column("company_address")]
    [StringLength(255)]
    public string? CompanyAddress { get; set; }
    
    [Column("fee_plan")]
    public int FeePlan { get; set; }
    
    [Column("promotion")]
    public int Promotion { get; set; }
    
    [Column("is_draft")]
    public bool IsDraft { get; set; }
    
    // relationships
    public virtual User User { get; set; }
    public virtual Course Course { get; set; }
    public virtual ICollection<StudentClass> StudentsClasses { get; set; }
    public virtual ICollection<Attendance> Attendances { get; set; }
    public virtual ICollection<StudentGrade> StudentGrades { get; set; }
    public virtual ICollection<GpaRecord> GpaRecords { get; set; }

}