namespace DeltaWebApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ilkkurulum : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ZDeltaTests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ZDeltaTests");
        }
    }
}
