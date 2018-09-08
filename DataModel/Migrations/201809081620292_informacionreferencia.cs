namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class informacionreferencia : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "InformacionReferencia_Numero", c => c.String(maxLength: 50, storeType: "nvarchar"));
            AddColumn("dbo.Facturas", "InformacionReferencia_Razon", c => c.String(maxLength: 180, storeType: "nvarchar"));
            AddColumn("dbo.Facturas", "InformacionReferencia_Codigo", c => c.Int());
            AddColumn("dbo.Facturas", "InformacionReferencia_FechaEmision", c => c.DateTime(precision: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facturas", "InformacionReferencia_FechaEmision");
            DropColumn("dbo.Facturas", "InformacionReferencia_Codigo");
            DropColumn("dbo.Facturas", "InformacionReferencia_Razon");
            DropColumn("dbo.Facturas", "InformacionReferencia_Numero");
        }
    }
}
