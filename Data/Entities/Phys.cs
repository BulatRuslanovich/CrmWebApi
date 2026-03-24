using System.ComponentModel.DataAnnotations.Schema;

namespace CrmWebApi.Data.Entities;


[Table("phys")]
public class Phys
{
    [Column("phys_id")]
    public int PhysId { get; set; }
    [Column("spec_id")]
    public int? SpecId { get; set; }
    [Column("phys_firstname")]
    public string? PhysFirstname { get; set; }
    [Column("phys_lastname")]
    public string PhysLastname { get; set; } = null!;
    [Column("phys_middlename")]
    public string? PhysMiddlename { get; set; }
    [Column("phys_phone")]
    public string? PhysPhone { get; set; }
    [Column("phys_email")]
    public string? PhysEmail { get; set; }
    [Column("phys_position")]
    public string? PhysPosition { get; set; }
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    public Spec? Spec { get; set; }
    public ICollection<PhysOrg> PhysOrgs { get; set; } = [];
}
