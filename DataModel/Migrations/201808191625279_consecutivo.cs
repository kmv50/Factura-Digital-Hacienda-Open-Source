namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class consecutivo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contribuyente_Consecutivos",
                c => new
                    {
                        Id_Contribuyente = c.Int(nullable: false),
                        Consecutivo_Facturas = c.Int(nullable: false),
                        Consecutivo_NotasCredito = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Contribuyente)
                .ForeignKey("dbo.Contribuyentes", t => t.Id_Contribuyente)
                .Index(t => t.Id_Contribuyente);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contribuyente_Consecutivos", "Id_Contribuyente", "dbo.Contribuyentes");
            DropIndex("dbo.Contribuyente_Consecutivos", new[] { "Id_Contribuyente" });
            DropTable("dbo.Contribuyente_Consecutivos");
        }
    }
}
