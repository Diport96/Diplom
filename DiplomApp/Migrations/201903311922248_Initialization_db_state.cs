namespace DiplomApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialization_db_state : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RegisteredDeviceInfoes",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        DeviceType = c.String(nullable: false),
                        RegisteredDate = c.DateTime(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RegisteredDeviceInfoes");
        }
    }
}
