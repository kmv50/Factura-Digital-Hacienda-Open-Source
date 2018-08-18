namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UbicacionesTabla : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ubicaciones",
                c => new
                    {
                        Id_Ubicacion = c.Int(nullable: false, identity: true),
                        Id_Provincia = c.Int(nullable: false),
                        Provincia = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Id_Canton = c.Int(nullable: false),
                        Canton = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Id_Distrito = c.Int(nullable: false),
                        Distrito = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        Id_Barrio = c.Int(nullable: false),
                        Barrio = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id_Ubicacion);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Ubicaciones");
        }
    }
}
