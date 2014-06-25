using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Data.Entity.Migrations.History;

namespace Pablo.Gallery.Models
{
	sealed class Configuration : DbConfiguration
	{
		public Configuration()
		{
			// general configuration
		}
	}

	public class GalleryContext : DbContext
	{
		public const string Schema = "gallery";

		public GalleryContext()
			: base("Gallery")
		{
			//var entity = ObjectContext.MetadataWorkspace.GetEntityContainer(ObjectContext.DefaultContainerName, DataSpace.CSpace)
			//	.BaseEntitySets.First(r => r.Name == "File");
		}

		public ObjectContext ObjectContext { get { return ((IObjectContextAdapter)this).ObjectContext; } }

		public string EscapeContains(string search)
		{
			// hack, npgsql doesn't escape strings for contains search
			return IsPostgres ? search.Replace(@"\", @"\\") : search;
		}

		public bool IsPostgres
		{
			get { return Database.Connection is Npgsql.NpgsqlConnection; }
		}

		public bool IsSqlServer
		{
			get { return Database.Connection is System.Data.SqlClient.SqlConnection; }
		}

		public DbSet<Pack> Packs { get; set; }

		public DbSet<File> Files { get; set; }

		public DbSet<FileContent> FileContents { get; set; }

		public DbSet<Category> Categories { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<Role> Roles { get; set; }

		public DbSet<UserOAuthMembership> UserOAuthMemberships { get; set; }

		public IQueryable<File> QueryFiles(string format = null, string type = null, string query = null)
		{
			return from f in Files
				where
				(format == null || f.Format.ToLower() == format.ToLower())
				&& (type == null || f.Type.ToLower() == type.ToLower())
				&& (query == null || f.FileName.ToLower().Contains(query.ToLower()))
				orderby f.Pack.Date descending, f.FileName
				select f;
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Pack>().
				HasMany(c => c.Categories).
				WithMany(p => p.Packs).
				Map(m =>
			{
				m.MapLeftKey("Pack_Id");
				m.MapRightKey("Category_Id");
				m.ToTable("Pack_Category", Schema);
			});

			modelBuilder.Entity<User>().
				HasMany(c => c.Roles).
				WithMany(p => p.Users).
				Map(m =>
			{
				m.MapLeftKey("User_Id");
				m.MapRightKey("Role_Id");
				m.ToTable("User_Role", Schema);
			});
		}
	}
}