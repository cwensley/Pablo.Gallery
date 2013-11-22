using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace Pablo.Gallery.Models
{
	internal sealed class Configuration : DbConfiguration
	{
		public Configuration()
		{
		}
	}

	public sealed class GalleryConfiguration : DbMigrationsConfiguration<GalleryContext>
	{
		public GalleryConfiguration()
		{
			AutomaticMigrationsEnabled = false;
		}
	}

	public class GalleryContext : DbContext
	{
		public GalleryContext()
			: base("Gallery")
		{
		}

		public DbSet<Pack> Packs { get; set; }
		public DbSet<File> Files { get; set; }
		public DbSet<Category> Categories { get; set; }
		//public DbSet<PackCategory> PackCategories { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Pack>().
			  HasMany(c => c.Categories).
			  WithMany(p => p.Packs).
			  Map(
			   m =>
			   {
				   m.MapLeftKey("Pack_Id");
				   m.MapRightKey("Category_Id");
				   m.ToTable("Pack_Category");
			   });
		}
	}
}