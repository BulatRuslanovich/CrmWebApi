using System.ComponentModel.DataAnnotations.Schema;

namespace CrmWebApi.Data.Entities;

[Table("activ")]
public class Activ
{
    [Column("activ_id")]
    public int ActivId { get; set; }
    [Column("usr_id")]
    public int UsrId { get; set; }
    [Column("org_id")]
    public int OrgId { get; set; }
    [Column("status_id")]
    public int StatusId { get; set; }
    [Column("activ_start")]
    public DateTimeOffset? ActivStart { get; set; }
    [Column("activ_end")]
    public DateTimeOffset? ActivEnd { get; set; }
    [Column("activ_description")]
    public string? ActivDescription { get; set; }
    [Column("activ_result")]
    public string? ActivResult { get; set; }
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    public Usr Usr { get; set; } = null!;
    public Org Org { get; set; } = null!;
    public Status Status { get; set; } = null!;
    public ICollection<ActivDrug> ActivDrugs { get; set; } = [];
}
