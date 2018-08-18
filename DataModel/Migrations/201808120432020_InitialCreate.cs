namespace ApiModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clientes",
                c => new
                    {
                        Id_Cliente = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 80, storeType: "nvarchar"),
                        Identificacion_Tipo = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Identificacion_Numero = c.String(maxLength: 20, storeType: "nvarchar"),
                        NombreComercial = c.String(maxLength: 80, storeType: "nvarchar"),
                        CorreoElectronico = c.String(maxLength: 60, storeType: "nvarchar"),
                        Provincia = c.Int(),
                        Canton = c.Int(),
                        Distrito = c.Int(),
                        Barrio = c.Int(),
                        OtrasSenas = c.String(maxLength: 160, storeType: "nvarchar"),
                        Telefono_Codigo = c.Int(),
                        Telefono_Numero = c.Int(),
                        Id_Contribuyente = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Cliente)
                .ForeignKey("dbo.Contribuyentes", t => t.Id_Contribuyente, cascadeDelete: true)
                .Index(t => t.Id_Contribuyente);
            
            CreateTable(
                "dbo.Contribuyentes",
                c => new
                    {
                        Id_Contribuyente = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false, maxLength: 80, storeType: "nvarchar"),
                        Identificacion_Tipo = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Identificacion_Numero = c.String(maxLength: 20, storeType: "nvarchar"),
                        NombreComercial = c.String(maxLength: 80, storeType: "nvarchar"),
                        CorreoElectronico = c.String(maxLength: 60, storeType: "nvarchar"),
                        Provincia = c.Int(nullable: false),
                        Canton = c.Int(nullable: false),
                        Distrito = c.Int(nullable: false),
                        Barrio = c.Int(nullable: false),
                        OtrasSenas = c.String(maxLength: 160, storeType: "nvarchar"),
                        Telefono_Codigo = c.Int(nullable: false),
                        Telefono_Numero = c.Int(nullable: false),
                        UsuarioHacienda = c.String(maxLength: 250, storeType: "nvarchar"),
                        ContrasenaHacienda = c.String(maxLength: 250, storeType: "nvarchar"),
                        Certificado = c.Binary(storeType: "blob"),
                        Contrasena_Certificado = c.String(maxLength: 20, storeType: "nvarchar"),
                        FechaExpiracion = c.DateTime(precision: 0),
                    })
                .PrimaryKey(t => t.Id_Contribuyente);
            
            CreateTable(
                "dbo.Facturas",
                c => new
                    {
                        Id_Factura = c.Int(nullable: false, identity: true),
                        Estado = c.Int(nullable: false),
                        Clave = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        NumeroConsecutivo = c.Int(nullable: false),
                        FechaEmision = c.DateTime(nullable: false, precision: 0),
                        Emisor_Nombre = c.String(nullable: false, maxLength: 80, storeType: "nvarchar"),
                        Emisor_Identificacion_Tipo = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Emisor_Identificacion_Numero = c.String(maxLength: 20, storeType: "nvarchar"),
                        Emisor_NombreComercial = c.String(maxLength: 80, storeType: "nvarchar"),
                        Emisor_Ubicacion_Provincia = c.Int(),
                        Emisor_Ubicacion_Canton = c.Int(),
                        Emisor_Ubicacion_Distrito = c.Int(),
                        Emisor_Ubicacion_Barrio = c.Int(),
                        Emisor_Ubicacion_OtrasSenas = c.String(maxLength: 160, storeType: "nvarchar"),
                        Emisor_Telefono_Codigo = c.Int(),
                        Emisor_Telefono_Numero = c.Int(),
                        Emisor_CorreoElectronico = c.String(nullable: false, maxLength: 60, storeType: "nvarchar"),
                        Receptor_Nombre = c.String(nullable: false, maxLength: 80, storeType: "nvarchar"),
                        Receptor_Identificacion_Tipo = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Receptor_Identificacion_Numero = c.String(maxLength: 20, storeType: "nvarchar"),
                        Receptor_NombreComercial = c.String(maxLength: 80, storeType: "nvarchar"),
                        Receptor_Ubicacion_Provincia = c.Int(),
                        Receptor_Ubicacion_Canton = c.Int(),
                        Receptor_Ubicacion_Distrito = c.Int(),
                        Receptor_Ubicacion_Barrio = c.Int(),
                        Receptor_Ubicacion_OtrasSenas = c.String(maxLength: 160, storeType: "nvarchar"),
                        Receptor_Telefono_Codigo = c.Int(),
                        Receptor_Telefono_Numero = c.Int(),
                        Receptor_CorreoElectronico = c.String(maxLength: 60, storeType: "nvarchar"),
                        CondicionVenta = c.String(nullable: false, maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        MedioPago = c.String(nullable: false, maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
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
                        Id_TipoDocumento = c.Int(nullable: false),
                        Email_Enviado = c.Boolean(nullable: false),
                        XML_Enviado = c.String(nullable: false, unicode: false, storeType: "text"),
                        XML_Respuesta = c.String(nullable: false, unicode: false, storeType: "text"),
                        Detalle = c.String(maxLength: 1500, storeType: "nvarchar"),
                        Log_Envio_Api = c.String(maxLength: 2500, storeType: "nvarchar"),
                        Ultimo_Log_Consulta_Api = c.String(maxLength: 2500, storeType: "nvarchar"),
                        Id_Contribuyente = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Factura)
                .ForeignKey("dbo.Contribuyentes", t => t.Id_Contribuyente, cascadeDelete: true)
                .Index(t => t.Id_Contribuyente);
            
            CreateTable(
                "dbo.Facturas_Detalles",
                c => new
                    {
                        Id_Factura_Detalle = c.Int(nullable: false, identity: true),
                        Codigo = c.String(maxLength: 20, storeType: "nvarchar"),
                        Unidad_Medida = c.String(nullable: false, maxLength: 15, storeType: "nvarchar"),
                        ProductoServicio = c.String(nullable: false, maxLength: 160, storeType: "nvarchar"),
                        PrecioUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tipo = c.Boolean(nullable: false),
                        Monto_Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Monto_Descuento = c.Decimal(precision: 18, scale: 2),
                        Naturaleza_Descuento = c.String(maxLength: 80, storeType: "nvarchar"),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Gravado = c.Boolean(nullable: false),
                        Monto_Total_Linea = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Id_Factura = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Factura_Detalle)
                .ForeignKey("dbo.Facturas", t => t.Id_Factura, cascadeDelete: true)
                .Index(t => t.Id_Factura);
            
            CreateTable(
                "dbo.Factura_Detalles_Impuestos",
                c => new
                    {
                        Id_Factura_Detalle = c.Int(nullable: false),
                        Impuesto_Codigo = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Impuesto_Tarifa = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Impuesto_TarifaReal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Impuesto_Monto = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Exento = c.Boolean(nullable: false),
                        Exoneracion_TipoDocumento = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Exoneracion_NumeroDocumento = c.String(maxLength: 17, unicode: false),
                        Exoneracion_NombreInstitucion = c.String(maxLength: 100, unicode: false),
                        Exoneracion_MontoImpuesto = c.Decimal(precision: 18, scale: 2),
                        Exoneracion_FechaEmision = c.DateTime(precision: 0),
                        Exoneracion_PorcentajeCompra = c.String(maxLength: 3, fixedLength: true, unicode: false, storeType: "char"),
                    })
                .PrimaryKey(t => t.Id_Factura_Detalle)
                .ForeignKey("dbo.Facturas_Detalles", t => t.Id_Factura_Detalle)
                .Index(t => t.Id_Factura_Detalle);
            
            CreateTable(
                "dbo.Productos",
                c => new
                    {
                        Id_Producto = c.Int(nullable: false, identity: true),
                        Codigo = c.String(maxLength: 20, storeType: "nvarchar"),
                        Unidad_Medida = c.String(nullable: false, maxLength: 15, storeType: "nvarchar"),
                        ProductoServicio = c.String(nullable: false, maxLength: 160, storeType: "nvarchar"),
                        PrecioUnitario = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tipo = c.Boolean(nullable: false),
                        Id_Contribuyente = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_Producto)
                .ForeignKey("dbo.Contribuyentes", t => t.Id_Contribuyente, cascadeDelete: true)
                .Index(t => t.Id_Contribuyente);
            
            CreateTable(
                "dbo.Productos_Impuestos",
                c => new
                    {
                        Id_Producto_Impuesto = c.Int(nullable: false),
                        Impuesto_Tipo = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Exento = c.Boolean(nullable: false),
                        Impuesto_Tarifa = c.Decimal(precision: 18, scale: 2),
                        Exoneracion_TipoDocumento = c.String(maxLength: 2, fixedLength: true, unicode: false, storeType: "char"),
                        Exoneracion_NumeroDocumento = c.String(maxLength: 17, storeType: "nvarchar"),
                        Exoneracion_NombreInstitucion = c.String(maxLength: 100, storeType: "nvarchar"),
                        Exoneracion_MontoImpuesto = c.Decimal(precision: 18, scale: 2),
                        Exoneracion_FechaEmision = c.DateTime(precision: 0),
                        Exoneracion_PorcentajeCompra = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id_Producto_Impuesto)
                .ForeignKey("dbo.Productos", t => t.Id_Producto_Impuesto)
                .Index(t => t.Id_Producto_Impuesto);
            
            CreateTable(
                "dbo.SMTP",
                c => new
                    {
                        Id_SMPT = c.Int(nullable: false, identity: true),
                        Url_Servidor = c.String(nullable: false, maxLength: 150, storeType: "nvarchar"),
                        Usuario = c.String(nullable: false, maxLength: 150, storeType: "nvarchar"),
                        Contrasena = c.String(nullable: false, maxLength: 150, storeType: "nvarchar"),
                        Puerto = c.Int(nullable: false),
                        Id_Contribuyente = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id_SMPT)
                .ForeignKey("dbo.Contribuyentes", t => t.Id_Contribuyente, cascadeDelete: true)
                .Index(t => t.Id_Contribuyente);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SMTP", "Id_Contribuyente", "dbo.Contribuyentes");
            DropForeignKey("dbo.Productos_Impuestos", "Id_Producto_Impuesto", "dbo.Productos");
            DropForeignKey("dbo.Productos", "Id_Contribuyente", "dbo.Contribuyentes");
            DropForeignKey("dbo.Factura_Detalles_Impuestos", "Id_Factura_Detalle", "dbo.Facturas_Detalles");
            DropForeignKey("dbo.Facturas_Detalles", "Id_Factura", "dbo.Facturas");
            DropForeignKey("dbo.Facturas", "Id_Contribuyente", "dbo.Contribuyentes");
            DropForeignKey("dbo.Clientes", "Id_Contribuyente", "dbo.Contribuyentes");
            DropIndex("dbo.SMTP", new[] { "Id_Contribuyente" });
            DropIndex("dbo.Productos_Impuestos", new[] { "Id_Producto_Impuesto" });
            DropIndex("dbo.Productos", new[] { "Id_Contribuyente" });
            DropIndex("dbo.Factura_Detalles_Impuestos", new[] { "Id_Factura_Detalle" });
            DropIndex("dbo.Facturas_Detalles", new[] { "Id_Factura" });
            DropIndex("dbo.Facturas", new[] { "Id_Contribuyente" });
            DropIndex("dbo.Clientes", new[] { "Id_Contribuyente" });
            DropTable("dbo.SMTP");
            DropTable("dbo.Productos_Impuestos");
            DropTable("dbo.Productos");
            DropTable("dbo.Factura_Detalles_Impuestos");
            DropTable("dbo.Facturas_Detalles");
            DropTable("dbo.Facturas");
            DropTable("dbo.Contribuyentes");
            DropTable("dbo.Clientes");
        }
    }
}
