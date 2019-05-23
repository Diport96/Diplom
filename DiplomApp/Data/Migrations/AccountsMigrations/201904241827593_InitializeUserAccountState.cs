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
                        Salt = c.Binary(nullable: false),
                        Key = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
       
        public override void Down()
        {
            DropTable("dbo.UserAccounts");
        }
    }
}
