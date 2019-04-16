namespace DiplomApp.Data.Migrations.DevicesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitializeDeviceState : DbMigration
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
                        PermitToConnection = c.Boolean(nullable: false),
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
