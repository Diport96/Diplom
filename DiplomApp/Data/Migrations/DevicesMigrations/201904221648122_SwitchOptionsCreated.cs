namespace DiplomApp.Data.Migrations.DevicesMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SwitchOptionsCreated : DbMigration
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
            
            CreateTable(
                "dbo.SwitchOptions",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Control = c.Int(nullable: false),
                        SensorId = c.String(),
                        DelayToSwitch = c.Int(),
                        ValueTo = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.RegisteredDeviceInfoes", t => t.ID)
                .Index(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SwitchOptions", "ID", "dbo.RegisteredDeviceInfoes");
            DropIndex("dbo.SwitchOptions", new[] { "ID" });
            DropTable("dbo.SwitchOptions");
            DropTable("dbo.RegisteredDeviceInfoes");
        }
    }
}
