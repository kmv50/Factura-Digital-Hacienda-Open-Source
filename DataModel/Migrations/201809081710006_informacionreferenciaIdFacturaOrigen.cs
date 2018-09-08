namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class informacionreferenciaIdFacturaOrigen : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas", "InformacionReferencia_IdFactura", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facturas", "InformacionReferencia_IdFactura");
        }
    }
}
