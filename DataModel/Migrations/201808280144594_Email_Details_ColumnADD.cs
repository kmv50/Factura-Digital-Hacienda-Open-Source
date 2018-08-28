namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Email_Details_ColumnADD : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SMTP", "Detalle_Email", c => c.String(maxLength: 150, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SMTP", "Detalle_Email");
        }
    }
}
