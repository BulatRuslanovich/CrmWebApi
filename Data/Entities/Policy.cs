using System.ComponentModel.DataAnnotations.Schema;

namespace CrmWebApi.Data.Entities;

[Table("policy")]
public class Policy
{
    [Column("policy_id")]
    public int PolicyId { get; set; }
    [Column("policy_name")]
    public string PolicyName { get; set; } = null!;
    [Column("is_deleted")]
    public bool IsDeleted { get; set; }

    public ICollection<UsrPolicy> UsrPolicies { get; set; } = [];
}
