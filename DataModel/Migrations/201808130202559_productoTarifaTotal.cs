namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productoTarifaTotal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Productos", "ImpuestosTarifaTotal", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Productos", "ImpuestosTarifaTotal");
        }
    }
}
