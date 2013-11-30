using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace Pablo.Gallery.Models
{
	sealed class Configuration : DbConfiguration
	{
		public Configuration()
		{
			// don't check or create database if it doesn't exist
			// hide (harmless) npgsql exceptions when debugging
			SetDatabaseInitializer<GalleryContext>(null);
		}
	}

	public sealed class GalleryConfiguration : DbMigrationsConfiguration<GalleryContext>
	{
		public GalleryConfiguration()
		{
			AutomaticMigrationsEnabled = true;
		}
	}

	public class GalleryContext : DbContext
	{
		public const string Schema = "gallery";

		public GalleryContext()
			: base("Gallery")
		{
		}

		public bool IsPostgres
		{
			get { return this.Database.Connection is Npgsql.NpgsqlConnection; }
		}

		public DbSet<Pack> Packs { get; set; }

		public DbSet<File> Files { get; set; }

		public DbSet<Category> Categories { get; set; }

		public DbSet<User> Users { get; set; }

		public DbSet<Role> Roles { get; set; }

		public DbSet<UserOAuthMembership> UserOAuthMemberships { get; set; }

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