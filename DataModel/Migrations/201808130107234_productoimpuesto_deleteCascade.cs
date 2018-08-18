namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productoimpuesto_deleteCascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Productos_Impuestos", "Id_Producto_Impuesto", "dbo.Productos");
            DropIndex("dbo.Productos_Impuestos", new[] { "Id_Producto_Impuesto" });
            DropPrimaryKey("dbo.Productos_Impuestos");
            AddColumn("dbo.Productos_Impuestos", "Producto_Id_Producto", c => c.Int(nullable: false));
            AlterColumn("dbo.Productos_Impuestos", "Id_Producto_Impuesto", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Productos_Impuestos", "Id_Producto_Impuesto");
            CreateIndex("dbo.Productos_Impuestos", "Producto_Id_Producto");
            AddForeignKey("dbo.Productos_Impuestos", "Producto_Id_Producto", "dbo.Productos", "Id_Producto", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Productos_Impuestos", "Producto_Id_Producto", "dbo.Productos");
            DropIndex("dbo.Productos_Impuestos", new[] { "Producto_Id_Producto" });
            DropPrimaryKey("dbo.Productos_Impuestos");
            AlterColumn("dbo.Productos_Impuestos", "Id_Producto_Impuesto", c => c.Int(nullable: false));
            DropColumn("dbo.Productos_Impuestos", "Producto_Id_Producto");
            AddPrimaryKey("dbo.Productos_Impuestos", "Id_Producto_Impuesto");
            CreateIndex("dbo.Productos_Impuestos", "Id_Producto_Impuesto");
            AddForeignKey("dbo.Productos_Impuestos", "Id_Producto_Impuesto", "dbo.Productos", "Id_Producto");
        }
    }
}
