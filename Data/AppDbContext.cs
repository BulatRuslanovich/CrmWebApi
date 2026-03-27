using CrmWebApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrmWebApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<OrgType> OrgTypes { get; set; }
	public DbSet<Status> Statuses { get; set; }
	public DbSet<Policy> Policies { get; set; }
	public DbSet<Organization> Orgs { get; set; }
	public DbSet<Spec> Specs { get; set; }
	public DbSet<Phys> Physes { get; set; }
	public DbSet<PhysOrg> PhysOrgs { get; set; }
	public DbSet<Drug> Drugs { get; set; }
	public DbSet<Usr> Usrs { get; set; }
	public DbSet<UsrPolicy> UsrPolicies { get; set; }
	public DbSet<Activ> Activs { get; set; }
	public DbSet<ActivDrug> ActivDrugs { get; set; }
	public DbSet<Refresh> Refreshes { get; set; }
	public DbSet<EmailToken> EmailTokens { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<PhysOrg>()
			.HasIndex(po => new { po.PhysId, po.OrgId })
			.IsUnique();

		modelBuilder.Entity<UsrPolicy>()
			.HasIndex(up => new { up.UsrId, up.PolicyId })
			.IsUnique();

		modelBuilder.Entity<ActivDrug>()
			.HasIndex(ad => new { ad.ActivId, ad.DrugId })
			.IsUnique();
	}
}
