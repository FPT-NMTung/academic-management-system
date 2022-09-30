using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("active_refresh_token")]
public class ActiveRefreshToken
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("refresh_token")]
    public string RefreshToken { get; set; }

    [Column("exp_date")]
    public DateTime ExpDate { get; set; }
    
    // relationships
    public virtual User User { get; set; }
}