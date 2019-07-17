namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nuevoscampos43 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contribuyente_ActividadesEconomicas",
                c => new
                    {
                        Codigo = c.Int(nullable: false, identity: true),
                        Estado = c.String(maxLength: 3, fixedLength: true, unicode: false, storeType: "char"),
                        Descripcion = c.String(maxLength: 250, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Codigo);
            
            AddColumn("dbo.Factura_Detalles_Impuestos", "CodigoTarifa", c => c.String(maxLength: 3, fixedLength: true, unicode: false, storeType: "char"));
            AddColumn("dbo.Factura_Detalles_Impuestos", "FactorIVA", c => c.Decimal(precision: 5, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Factura_Detalles_Impuestos", "FactorIVA");
            DropColumn("dbo.Factura_Detalles_Impuestos", "CodigoTarifa");
            DropTable("dbo.Contribuyente_ActividadesEconomicas");
        }
    }
}
