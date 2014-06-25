namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class indexes : DbMigration
    {
        public override void Up()
        {
            CreateIndex("gallery.Pack", "Name", unique: true, name: "ux_Pack_Name");
            CreateIndex("gallery.User", "UserName", unique: true, name: "ux_User_UserName");
        }
        
        public override void Down()
        {
            DropIndex("gallery.User", "ux_User_UserName");
            DropIndex("gallery.Pack", "ux_Pack_Name");
        }
    }
}
