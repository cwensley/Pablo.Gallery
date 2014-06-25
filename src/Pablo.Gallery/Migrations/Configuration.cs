namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Pablo.Gallery.Models.GalleryContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Pablo.Gallery.Models.GalleryContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //

			// create admin role
			context.Roles.AddOrUpdate(r => r.Name,
				new Models.Role { Name = "Admin" }
				);
        }
    }
}
