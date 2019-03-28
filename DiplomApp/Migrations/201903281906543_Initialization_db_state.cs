namespace DiplomApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialization_db_state : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Controllers",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Controllers");
        }
    }
}
