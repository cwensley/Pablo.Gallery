namespace Pablo.Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "gallery.Category",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "gallery.Pack",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 256, fixedLength: true),
                        FileName = c.String(maxLength: 1073741823, fixedLength: true),
                        Date = c.DateTime(),
                        Thumbnail_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gallery.File", t => t.Thumbnail_Id)
                .Index(t => t.Thumbnail_Id);
            
            CreateTable(
                "gallery.File",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 1073741823, fixedLength: true),
                        FileName = c.String(maxLength: 1073741823, fixedLength: true),
                        Format = c.String(maxLength: 20, fixedLength: true),
                        Type = c.String(maxLength: 20, fixedLength: true),
                        Order = c.Int(),
                        Width = c.Int(),
                        Height = c.Int(),
                        Pack_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("gallery.Pack", t => t.Pack_Id, cascadeDelete: true)
                .Index(t => t.Pack_Id);
            
            CreateTable(
                "gallery.File_Content",
                c => new
                    {
                        File_Id = c.Int(nullable: false),
                        Text = c.String(maxLength: 1073741823, fixedLength: true),
                    })
                .PrimaryKey(t => t.File_Id)
                .ForeignKey("gallery.File", t => t.File_Id)
                .Index(t => t.File_Id);
            
            CreateTable(
                "gallery.Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "gallery.User",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreateDate = c.DateTime(),
                        LastLoginDate = c.DateTime(),
                        UserName = c.String(maxLength: 1073741823, fixedLength: true),
                        Email = c.String(maxLength: 1073741823, fixedLength: true),
                        Alias = c.String(maxLength: 1073741823, fixedLength: true),
                        ConfirmationToken = c.String(maxLength: 128, fixedLength: true),
                        IsConfirmed = c.Boolean(nullable: false),
                        LastPasswordFailureDate = c.DateTime(),
                        Password = c.String(maxLength: 128, fixedLength: true),
                        PasswordFailuresSinceLastSuccess = c.Int(nullable: false),
                        PasswordChangedDate = c.DateTime(),
                        PasswordSalt = c.String(maxLength: 1073741823, fixedLength: true),
                        PasswordVerificationToken = c.String(maxLength: 128, fixedLength: true),
                        PasswordVerificationExpiryDate = c.DateTime(),
                        PasswordQuestion = c.String(maxLength: 1073741823, fixedLength: true),
                        PasswordAnswer = c.String(maxLength: 1073741823, fixedLength: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "gallery.User_OAuthMembership",
                c => new
                    {
                        Provider = c.String(nullable: false, maxLength: 30, fixedLength: true),
                        ProviderUserId = c.String(nullable: false, maxLength: 100, fixedLength: true),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.Provider, t.ProviderUserId })
                .ForeignKey("gallery.User", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "gallery.Pack_Category",
                c => new
                    {
                        Pack_Id = c.Int(nullable: false),
                        Category_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Pack_Id, t.Category_Id })
                .ForeignKey("gallery.Pack", t => t.Pack_Id, cascadeDelete: true)
                .ForeignKey("gallery.Category", t => t.Category_Id, cascadeDelete: true)
                .Index(t => t.Pack_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "gallery.User_Role",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        Role_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Role_Id })
                .ForeignKey("gallery.User", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("gallery.Role", t => t.Role_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Role_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("gallery.User_Role", "Role_Id", "gallery.Role");
            DropForeignKey("gallery.User_Role", "User_Id", "gallery.User");
            DropForeignKey("gallery.User_OAuthMembership", "User_Id", "gallery.User");
            DropForeignKey("gallery.Pack", "Thumbnail_Id", "gallery.File");
            DropForeignKey("gallery.File", "Pack_Id", "gallery.Pack");
            DropForeignKey("gallery.File_Content", "File_Id", "gallery.File");
            DropForeignKey("gallery.Pack_Category", "Category_Id", "gallery.Category");
            DropForeignKey("gallery.Pack_Category", "Pack_Id", "gallery.Pack");
            DropIndex("gallery.User_Role", new[] { "Role_Id" });
            DropIndex("gallery.User_Role", new[] { "User_Id" });
            DropIndex("gallery.Pack_Category", new[] { "Category_Id" });
            DropIndex("gallery.Pack_Category", new[] { "Pack_Id" });
            DropIndex("gallery.User_OAuthMembership", new[] { "User_Id" });
            DropIndex("gallery.File_Content", new[] { "File_Id" });
            DropIndex("gallery.File", new[] { "Pack_Id" });
            DropIndex("gallery.Pack", new[] { "Thumbnail_Id" });
            DropTable("gallery.User_Role");
            DropTable("gallery.Pack_Category");
            DropTable("gallery.User_OAuthMembership");
            DropTable("gallery.User");
            DropTable("gallery.Role");
            DropTable("gallery.File_Content");
            DropTable("gallery.File");
            DropTable("gallery.Pack");
            DropTable("gallery.Category");
        }
    }
}
