using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Context.AmsModels;

[Table("user")]
public class User
{
    public User()
    {
        this.ActiveRefreshTokens = new HashSet<ActiveRefreshToken>();
    }

    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("province_id")]
    public int ProvinceId { get; set; }
    
    [Column("district_id")]
    public int DistrictId { get; set; }
    
    [Column("ward_id")]
    public int WardId { get; set; }
    
    [Column("center_id")]
    public int CenterId { get; set; }

    [Column("gender_id")] 
    public int GenderId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }
    
    [Column("first_name")]
    [StringLength(255)]
    public string FirstName{ get; set; }
    
    [Column("last_name")]
    [StringLength(255)]
    public string LastName{ get; set; }
    
    [Column("avatar")]
    [StringLength(1000)]
    public string? Avatar { get; set; }
    
    [Column("mobile_phone")]
    [StringLength(15)]
    public string MobilePhone { get; set; }
    
    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; }
    
    [Column("email_organization")]
    [StringLength(255)]
    public string EmailOrganization { get; set; }
    
    [Column("birthday", TypeName = "date")]
    public DateTime Birthday { get; set; }

    [Column("citizen_identity_card_no")]
    [StringLength(255)]
    public string CitizenIdentityCardNo { get; set; }
    
    [Column("citizen_identity_card_published_date", TypeName = "date")]
    public DateTime CitizenIdentityCardPublishedDate { get; set; }
    
    [Column("citizen_identity_card_published_place")]
    [StringLength(255)]
    public string CitizenIdentityCardPublishedPlace { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // relationships
    public virtual Province Province { get; set; }
    public virtual District District { get; set; }
    public virtual Ward Ward { get; set; }
    public virtual Center Center { get; set; }
    public virtual Gender Gender { get; set; }
    public virtual Role Role { get; set; }
    public virtual Admin Admin { get; set; }
    public virtual Sro Sro { get; set; }
    public virtual Teacher Teacher { get; set; }
    public virtual Student Student { get; set; }
    public virtual ICollection<ActiveRefreshToken> ActiveRefreshTokens { get; set; }
    
}