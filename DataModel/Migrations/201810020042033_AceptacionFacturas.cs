namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AceptacionFacturas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facturas_Resoluciones", "Resolucion", c => c.Int(nullable: false));
            AddColumn("dbo.Facturas_Resoluciones", "DetalleResolucion", c => c.String(maxLength: 80, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facturas_Resoluciones", "DetalleResolucion");
            DropColumn("dbo.Facturas_Resoluciones", "Resolucion");
        }
    }
}
