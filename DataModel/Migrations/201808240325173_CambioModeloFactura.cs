namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioModeloFactura : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "HaciendaDetalle", c => c.String(maxLength: 2500, storeType: "nvarchar"));
            DropColumn("dbo.Facturas", "Log_Envio_Api");
            DropColumn("dbo.Facturas", "Ultimo_Log_Consulta_Api");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Facturas", "Ultimo_Log_Consulta_Api", c => c.String(maxLength: 2500, storeType: "nvarchar"));
            AddColumn("dbo.Facturas", "Log_Envio_Api", c => c.String(maxLength: 2500, storeType: "nvarchar"));
            DropColumn("dbo.Facturas", "HaciendaDetalle");
        }
    }
}
