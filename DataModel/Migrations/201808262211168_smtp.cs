namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class smtp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SMTP", "SSL", c => c.Boolean(nullable: false));
            DropColumn("dbo.SMTP", "Tipo_Seguridad");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SMTP", "Tipo_Seguridad", c => c.Int(nullable: false));
            DropColumn("dbo.SMTP", "SSL");
        }
    }
}
