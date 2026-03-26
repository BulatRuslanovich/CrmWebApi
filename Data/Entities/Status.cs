using System.ComponentModel.DataAnnotations.Schema;

namespace CrmWebApi.Data.Entities;


[Table("status")]
public class Status
{
	[Column("status_id")]
	public int StatusId { get; set; }
	[Column("status_name")]
	public string StatusName { get; set; } = null!;
	[Column("is_deleted")]
	public bool IsDeleted { get; set; }

	public ICollection<Activ> Activs { get; set; } = [];
}
