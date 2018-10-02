namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AceptacionFacturasConcecutivo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contribuyente_Consecutivos", "Consecutivo_Confirmacion", c => c.Int(nullable: false));
            Sql("update Contribuyente_Consecutivos set Consecutivo_Confirmacion = 1");
            AlterColumn("dbo.Facturas_Resoluciones", "NumeroConsecutivo", c => c.String(nullable: false, maxLength: 20, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Facturas_Resoluciones", "NumeroConsecutivo", c => c.Int(nullable: false));
            DropColumn("dbo.Contribuyente_Consecutivos", "Consecutivo_Confirmacion");
        }
    }
}
