namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FacturasResolucion : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Facturas_Resoluciones",
                c => new
                    {
                        Id_Factura_Resolucion = c.Int(nullable: false, identity: true),
                        Estado = c.Int(nullable: false),
                        Clave = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        NumeroConsecutivo = c.Int(nullable: false),
                        Fecha_Documento_Origen = c.DateTime(nullable: false, precision: 0),
                        Fecha_Documento = c.DateTime(nullable: false, precision: 0),
                        Receptor_Identificacion = c.String(maxLength: 30, storeType: "nvarchar"),
                        Receptor__Nombre = c.String(maxLength: 80, storeType: "nvarchar"),
                        Emisor_Nombre = c.String(nullable: false, maxLength: 80, storeType: "nvarchar"),
                        Emisor_Identificacion_Tipo = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Emisor_Identificacion_Numero = c.String(maxLength: 20, storeType: "nvarchar"),
                        Emisor_NombreComercial = c.String(maxLength: 80, storeType: "nvarchar"),
                        Emisor_Telefono_Numero = c.String(maxLength: 25, storeType: "nvarchar"),
                        Emisor_CorreoElectronico = c.String(nullable: false, maxLength: 60, storeType: "nvarchar"),
                        CondicionVenta = c.String(nullable: false, maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        MedioPago = c.String(nullable: false, maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        TipoDocumentoOrigen = c.Int(nullable: false),
                        TipoCambio = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Codigo_Moneda = c.String(nullable: false, maxLength: 3, fixedLength: true, unicode: false, storeType: "char"),
                        TotalServGravados = c.Decimal(precision: 18, scale: 2),
                        TotalServExentos = c.Decimal(precision: 18, scale: 2),
                        TotalMercanciasGravadas = c.Decimal(precision: 18, scale: 2),
                        TotalMercanciasExentas = c.Decimal(precision: 18, scale: 2),
                        TotalGravado = c.Decimal(precision: 18, scale: 2),
                        TotalExento = c.Decimal(precision: 18, scale: 2),
                        TotalVenta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalDescuentos = c.Decimal(precision: 18, scale: 2),
                        TotalVentaNeta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalImpuesto = c.Decimal(precision: 18, scale: 2),
                        TotalComprobante = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Email_Enviado = c.Boolean(nullable: false),
                        XML_Subido = c.String(unicode: false, storeType: "text"),
                        XML_Enviado = c.String(unicode: false, storeType: "text"),
                        XML_Respuesta = c.String(unicode: false, storeType: "text"),
                        Id_Contribuyente = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Factura_Resolucion)
                .ForeignKey("dbo.Contribuyentes", t => t.Id_Contribuyente, cascadeDelete: true)
                .Index(t => t.Id_Contribuyente);
            
            CreateTable(
                "dbo.Factura_Resolucion_Detalle",
                c => new
                    {
                        Id_Factura_Resolucion_Detalle = c.Int(nullable: false, identity: true),
                        Codigo = c.String(maxLength: 20, storeType: "nvarchar"),
                        Cantidad = c.Int(nullable: false),
                        ProductoServicio = c.String(nullable: false, maxLength: 160, storeType: "nvarchar"),
                        PrecioUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Monto_Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Monto_Descuento = c.Decimal(precision: 18, scale: 2),
                        Naturaleza_Descuento = c.String(maxLength: 80, storeType: "nvarchar"),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gravado = c.Boolean(nullable: false),
                        Impuesto_Monto = c.Decimal(precision: 18, scale: 2),
                        Monto_Total_Linea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Id_Factura_Resolucion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Factura_Resolucion_Detalle)
                .ForeignKey("dbo.Facturas_Resoluciones", t => t.Id_Factura_Resolucion, cascadeDelete: true)
                .Index(t => t.Id_Factura_Resolucion);
            
            CreateTable(
                "dbo.Factura_Resolucion_Detalle_Impuesto",
                c => new
                    {
                        Id_Factura_Resolucion_Detalle_Impuesto = c.Int(nullable: false, identity: true),
                        Impuesto_Codigo = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Impuesto_Tarifa = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Impuesto_Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Id_Factura_Resolucion_Detalle = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Factura_Resolucion_Detalle_Impuesto)
                .ForeignKey("dbo.Factura_Resolucion_Detalle", t => t.Id_Factura_Resolucion_Detalle, cascadeDelete: true)
                .Index(t => t.Id_Factura_Resolucion_Detalle);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Factura_Resolucion_Detalle", "Id_Factura_Resolucion", "dbo.Facturas_Resoluciones");
            DropForeignKey("dbo.Factura_Resolucion_Detalle_Impuesto", "Id_Factura_Resolucion_Detalle", "dbo.Factura_Resolucion_Detalle");
            DropForeignKey("dbo.Facturas_Resoluciones", "Id_Contribuyente", "dbo.Contribuyentes");
            DropIndex("dbo.Factura_Resolucion_Detalle_Impuesto", new[] { "Id_Factura_Resolucion_Detalle" });
            DropIndex("dbo.Factura_Resolucion_Detalle", new[] { "Id_Factura_Resolucion" });
            DropIndex("dbo.Facturas_Resoluciones", new[] { "Id_Contribuyente" });
            DropTable("dbo.Factura_Resolucion_Detalle_Impuesto");
            DropTable("dbo.Factura_Resolucion_Detalle");
            DropTable("dbo.Facturas_Resoluciones");
        }
    }
}
