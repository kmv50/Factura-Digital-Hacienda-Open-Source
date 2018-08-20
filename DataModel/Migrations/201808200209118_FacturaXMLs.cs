namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FacturaXMLs : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Facturas", "XML_Enviado", c => c.String(unicode: false, storeType: "text"));
            AlterColumn("dbo.Facturas", "XML_Respuesta", c => c.String(unicode: false, storeType: "text"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Facturas", "XML_Respuesta", c => c.String(nullable: false, unicode: false, storeType: "text"));
            AlterColumn("dbo.Facturas", "XML_Enviado", c => c.String(nullable: false, unicode: false, storeType: "text"));
        }
    }
}
