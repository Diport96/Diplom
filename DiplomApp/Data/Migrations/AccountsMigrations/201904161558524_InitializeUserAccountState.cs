namespace DiplomApp.Data.Migrations.AccountsMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitializeUserAccountState : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAccounts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false),
                        Salt = c.Binary(),
                        Key = c.Binary(),
                    })
                .PrimaryKey(t => t.ID)
                .Index(t => t.Login, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserAccounts", new[] { "Login" });
            DropTable("dbo.UserAccounts");
        }
    }
}
