namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ErroresSistema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Errores_Sistema",
                c => new
                    {
                        Fecha = c.DateTime(nullable: false, precision: 0),
                        Mensaje = c.String(unicode: false),
                        Stacktrace = c.String(unicode: false),
                        Inner_Mensaje = c.String(unicode: false),
                        Inner_Stacktrace = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Fecha);
            
            AddColumn("dbo.SMTP", "Tipo_Seguridad", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SMTP", "Tipo_Seguridad");
            DropTable("dbo.Errores_Sistema");
        }
    }
}
