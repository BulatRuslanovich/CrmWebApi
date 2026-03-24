using System.ComponentModel.DataAnnotations.Schema;

namespace CrmWebApi.Data.Entities;


[Table("org")]
public class Org
{
    [Column("org_id")]
    public int OrgId { get; set; }
    [Column("org_type_id")]
    public int OrgTypeId { get; set; }
    [Column("org_name")]
    public string OrgName { get; set; } = null!;
    [Column("org_inn")]
    public string? OrgInn { get; set; }
    [Column("org_latitude")]
    public double? OrgLatitude { get; set; }
    [Column("org_longitude")]
    public double? OrgLongitude { get; set; }
    [Column("org_address")]
    public string? OrgAddress { get; set; }
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    public OrgType OrgType { get; set; } = null!;
    public ICollection<PhysOrg> PhysOrgs { get; set; } = [];
    public ICollection<Activ> Activs { get; set; } = [];
}
