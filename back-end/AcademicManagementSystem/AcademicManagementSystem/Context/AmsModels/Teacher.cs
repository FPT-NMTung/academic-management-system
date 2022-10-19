﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("teacher")]
public class Teacher
{
    [Key]
    [Column("user_id")]
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [Column("teacher_type_id")]
    public int TeacherTypeId { get; set; }
    
    [Column("working_time_id")]
    public int WorkingTimeId { get; set; }
    
    [Column("nickname")]
    [StringLength(255)]
    public string? Nickname { get; set; }
    
    [Column("company_address")]
    public string? CompanyAddress { get; set; }
    
    [Column("start_working_date")]
    public DateTime StartWorkingDate { get; set; }
    
    [Column("salary")]
    public decimal? Salary { get; set; }
    
    //Unique Key
    [Column("tax_code")]
    [StringLength(10)]
    public string? TaxCode { get; set; }

    // relationships
    public virtual User User { get; set; }

    public virtual TeacherType TeacherType { get; set; }
    
    public virtual WorkingTime WorkingTime { get; set; }
        
}