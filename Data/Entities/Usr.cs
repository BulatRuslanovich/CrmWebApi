using System.ComponentModel.DataAnnotations.Schema;

namespace CrmWebApi.Data.Entities;

[Table("usr")]
public class Usr
{
    [Column("usr_id")]
    public int UsrId { get; set; }
    [Column("usr_firstname")]
    public string? UsrFirstname { get; set; }
    [Column("usr_lastname")]
    public string? UsrLastname { get; set; }
    [Column("usr_email")]
    public string? UsrEmail { get; set; }
    [Column("usr_phone")]
    public string? UsrPhone { get; set; }
    [Column("usr_login")]
    public string UsrLogin { get; set; } = null!;
    [Column("usr_password_hash")]
    public string UsrPasswordHash { get; set; } = null!;
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    public ICollection<UsrPolicy> UsrPolicies { get; set; } = [];
    public ICollection<Activ> Activs { get; set; } = [];
    public ICollection<Refresh> Refreshes { get; set; } = [];
}
