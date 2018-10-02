using DataModel.EF;
using FacturaElectronica_V_4_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Hacienda_Comunication
{
    public class ConvertirFacturaXml_FacturaDB : ILog
    {
        public Factura_Resolucion Convertir(FacturaElectronica factura , string XMl = null)
        {
            Factura_Resolucion fac = new Factura_Resolucion()
            {
                Clave = factura.Clave,
                CondicionVenta = factura.CondicionVenta.GetXmlValue(),
                Email_Enviado = false,
                TipoDocumentoOrigen = (int)Tipo_documento.Factura_electrónica,
                Estado = (int)EstadoComprobante.Enviado,
                Fecha_Documento_Origen = factura.FechaEmision                
            };

            if (factura.MedioPago != null && factura.MedioPago.Length > 0)
                fac.MedioPago = factura.MedioPago[0].GetXmlValue();

            LoadEmisor(ref fac,factura);
            LoadResumenFactura(ref fac, factura);    
            LoadDatosReceptor(ref fac, factura);
            LoadBody(ref fac, factura);
            return fac;
        }

        private Factura_Resolucion_Detalle CargarLinea(FacturaElectronicaLineaDetalle linea)
        {
            try
            {
                Factura_Resolucion_Detalle lineaDB = new Factura_Resolucion_Detalle();
                lineaDB.Cantidad = Convert.ToInt32(linea.Cantidad);
                if(lineaDB.Codigo != null && linea.Codigo.Length > 0)                
                    lineaDB.Codigo = linea.Codigo[0].Codigo;

                if (linea.MontoDescuentoSpecified)
                {
                    lineaDB.Monto_Descuento = linea.MontoDescuento;
                    lineaDB.Naturaleza_Descuento = linea.NaturalezaDescuento;
                }

                lineaDB.Monto_Total = linea.MontoTotal;
                lineaDB.Monto_Total_Linea = linea.MontoTotalLinea;
                lineaDB.PrecioUnitario = linea.PrecioUnitario;
                lineaDB.SubTotal = linea.SubTotal;
                lineaDB.ProductoServicio = linea.Detalle;
                lineaDB.Gravado = false;

                if (linea.Impuesto != null && linea.Impuesto.Length > 0)
                {
                    decimal MontoImpuesto = 0;
                    List<Factura_Resolucion_Detalle_Impuesto> impuestos = new List<Factura_Resolucion_Detalle_Impuesto>();
                    foreach (ImpuestoType im in linea.Impuesto)
                    {
                        MontoImpuesto += im.Monto;
                        impuestos.Add(new Factura_Resolucion_Detalle_Impuesto() {
                            Impuesto_Codigo = im.Codigo.GetXmlValue(),
                            Impuesto_Monto = im.Monto,
                            Impuesto_Tarifa = im.Tarifa
                        });  
                    }

                    if(impuestos.Count > 0)
                    {
                        lineaDB.Gravado = true;
                        lineaDB.Impuesto_Monto = MontoImpuesto;
                        lineaDB.Factura_Resolucion_Detalle_Impuesto = impuestos;
                    }
                       
                }

                return lineaDB;
            }
            catch(Exception ex)
            {
                this.LogError(ex);
                return null;
            }
        }

        private void LoadBody(ref Factura_Resolucion fac, FacturaElectronica factura)
        {
            FacturaElectronicaLineaDetalle[] DetallesServicios = factura.DetalleServicio;
            if (DetallesServicios != null && DetallesServicios.Length > 0)
            {
                List<Factura_Resolucion_Detalle> Detalles = new List<Factura_Resolucion_Detalle>();
                foreach (FacturaElectronicaLineaDetalle nodo in DetallesServicios)
                {
                    Factura_Resolucion_Detalle result = CargarLinea(nodo);
                    if(result != null)
                        Detalles.Add(result);
                }
                fac.Factura_Resolucion_Detalle = Detalles;
            }
        }


        private void LoadDatosReceptor(ref Factura_Resolucion fac, FacturaElectronica factura)
        {
            if(factura.Receptor != null)
            {
                fac.Receptor__Nombre = factura.Receptor.Nombre;
                if(factura.Receptor.Identificacion != null)                
                    fac.Receptor_Identificacion = factura.Receptor.Identificacion.Numero;                
                else                
                    fac.Receptor_Identificacion = factura.Receptor.IdentificacionExtranjero;                
            }
        }


        private void LoadResumenFactura(ref Factura_Resolucion fac, FacturaElectronica factura)
        {
            FacturaElectronicaResumenFactura Resumen = factura.ResumenFactura;
            if(Resumen != null)
            {
                if (Resumen.CodigoMonedaSpecified)                
                    fac.Codigo_Moneda = Resumen.CodigoMoneda.ToString();
                
                if (Resumen.TipoCambioSpecified)                
                    fac.TipoCambio = Resumen.TipoCambio;               

                if(Resumen.TotalDescuentosSpecified)
                    fac.TotalDescuentos = Resumen.TotalDescuentos ;

                if(Resumen.TotalExentoSpecified)
                    fac.TotalExento = Resumen.TotalExento;

                if(Resumen.TotalGravadoSpecified)
                    fac.TotalGravado = Resumen.TotalGravado;

                if(Resumen.TotalImpuestoSpecified)
                    fac.TotalImpuesto = Resumen.TotalImpuesto;

                if(Resumen.TotalMercanciasExentasSpecified)
                    fac.TotalMercanciasExentas = Resumen.TotalMercanciasExentas;

                if(Resumen.TotalMercanciasGravadasSpecified)
                    fac.TotalMercanciasGravadas = Resumen.TotalMercanciasGravadas;

                if (Resumen.TotalServExentosSpecified)
                    fac.TotalServExentos = Resumen.TotalServExentos;

                fac.TotalVenta = Resumen.TotalVenta;
                fac.TotalVentaNeta = Resumen.TotalVentaNeta;
                fac.TotalComprobante = Resumen.TotalComprobante;               

            }
        }

        private void LoadEmisor(ref Factura_Resolucion fac , FacturaElectronica factura)
        {
            if(factura.Emisor != null)
            {
               EmisorType Emi = factura.Emisor;
               fac.Emisor_CorreoElectronico = Emi.CorreoElectronico;
               if(Emi.Identificacion != null)
                {
                    fac.Emisor_Identificacion_Tipo = Emi.Identificacion.Tipo.GetXmlValue();
                    fac.Emisor_Identificacion_Numero = Emi.Identificacion.Numero;
                }

                fac.Emisor_Nombre = Emi.Nombre;
                fac.Emisor_NombreComercial = Emi.NombreComercial;
                if(Emi.Telefono != null)
                {
                    fac.Emisor_Telefono_Numero = Emi.Telefono.NumTelefono;
                }                
            }
        }
    }
}
