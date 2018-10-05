using DataModel;
using DataModel.EF;
using HiQPdf;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace FacturaDigital.FacturaPDF
{
    public class FacturaElectronicaPDF : ILog
    {
        private string IfStringIsNull(string a, string b = "No se indica")
        {
            if (string.IsNullOrEmpty(a))
            {
                if (string.IsNullOrEmpty(b))
                {
                    return "No se indica";
                }

                return b;
            }
            return a;
        }

        private string FullAddress(Factura fac)
        {
            string Address = string.Empty;
            try
            {

                using (db_FacturaDigital db = new db_FacturaDigital())
                {
                    Ubicacion ub = db.Ubicaciones.FirstOrDefault(q =>
                   q.Id_Barrio == fac.Emisor_Ubicacion_Barrio.Value
                   && q.Id_Provincia == fac.Emisor_Ubicacion_Provincia
                   && q.Id_Canton == fac.Emisor_Ubicacion_Canton
                   && q.Id_Distrito == fac.Emisor_Ubicacion_Distrito);

                    Address = ub.Provincia + " " + ub.Canton + " " + ub.Distrito + " " + ub.Barrio + " " + fac.Receptor_Ubicacion_OtrasSenas;
                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
            }

            return Address;
        }

        public string CrearFactura(Factura fac)
        {
            try
            {
                HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
                htmlToPdfConverter.Document.PageSize = PdfPageSize.A4;
                htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
                htmlToPdfConverter.Document.Margins = new PdfMargins(0);
                htmlToPdfConverter.Document.FitPageHeight = true;
                string Html = FacturaDigital.Properties.Resources.factura;

                string Consecutivo = "";
                try
                {
                    Consecutivo = new ConsecutivoHacienda()
                    {
                        CasaMatriz = fac.CasaMatriz,
                        NumeracionConsecutiva = fac.NumeroConsecutivo,
                        PuntoVenta = fac.PuntoVenta,
                        TipoDocumento = (Tipo_documento)fac.Id_TipoDocumento
                    }.ToString();
                }
                catch (Exception ex)
                {
                    this.LogError(ex);
                }

                string TelefonoEmisor = null;
                if (fac.Emisor_Telefono_Numero.HasValue)
                {
                    TelefonoEmisor = fac.Emisor_Telefono_Numero.ToString();
                }

                string TelefonoReceptor = null;
                if (fac.Receptor_Telefono_Numero.HasValue)
                {
                    TelefonoReceptor = fac.Receptor_Telefono_Numero.ToString();
                }

                CultureInfo formatstr = new CultureInfo("es-CR");
                formatstr.NumberFormat.CurrencySymbol = "₡";
                formatstr.NumberFormat.CurrencyDecimalDigits = 2;

                StringBuilder Body = new StringBuilder(); ;
                foreach (Factura_Detalle Item in fac.Factura_Detalle)
                {
                    Body.Append("<tr>");

                    Body.Append("<td>" + Item.Codigo + "</td>");
                    Body.Append("<td>" + Item.ProductoServicio + "</td>");
                    Body.Append("<td>" + Item.Cantidad.ToString() + "</td>");
                    Body.Append("<td>" + Item.PrecioUnitario.ToString("C", formatstr) + "</td>");
                    Body.Append("<td>" + (Item.Impuesto_Monto ?? 0).ToString("C", formatstr) + "</td>");
                    Body.Append("<td>" + (Item.Monto_Descuento ?? 0).ToString("C", formatstr) + "</td>");
                    Body.Append("<td>" + Item.Monto_Total_Linea.ToString("C", formatstr) + "</td>");

                    Body.Append("</tr>");
                }


                Html = Html.Replace("[Consecutivo]", Consecutivo)
                .Replace("[Fecha]", fac.Fecha_Emision_Documento.ToString("yyyy/MM/dd/ hh:mm tt"))
                .Replace("[Nombre_Vendedor]", IfStringIsNull(fac.Emisor_NombreComercial, fac.Emisor_Nombre))
                .Replace("[Identificacion_Vendedor]", IfStringIsNull(fac.Emisor_Identificacion_Numero))
                .Replace("[Email_Vendedor]", IfStringIsNull(fac.Emisor_CorreoElectronico))
                .Replace("[Telefono_Vendedor]", IfStringIsNull(TelefonoEmisor))
                .Replace("[Nombre_Comprador]", IfStringIsNull(fac.Receptor_NombreComercial, fac.Receptor_Nombre))
                .Replace("[Email_Comprador]", IfStringIsNull(fac.Receptor_CorreoElectronico))
                .Replace("[Telefono_Comprador]", IfStringIsNull(TelefonoReceptor))
                .Replace("[Medio_Pago]", Utilides.GetMedioDePagoFullName(fac.MedioPago))
                .Replace("[Condicion_Venta]", Utilides.GetCondicionVentaFullName(fac.CondicionVenta))
                .Replace("[Direccion_Vendedor]", FullAddress(fac))
                .Replace("[Clave]", fac.Clave)
                .Replace("[BodyFactura]", Body.ToString())
                .Replace("[SubTotal]", fac.TotalVenta.ToString("C", formatstr))
                .Replace("[Descuento]", (fac.TotalDescuentos ?? 0).ToString("C", formatstr))
                .Replace("[Impuesto]", (fac.TotalImpuesto ?? 0).ToString("C", formatstr))
                .Replace("[Total]", fac.TotalComprobante.ToString("C", formatstr));


                byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory(Html, null);
                string NewPdfName = Guid.NewGuid().ToString() + ".pdf";
                File.WriteAllBytes(NewPdfName, pdfBuffer);
                return NewPdfName;
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al crear el PDF", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }


        public void MostrarFactura(string FacturaNamePDF)
        {

            try
            {
                foreach (string pdfFile in Directory.GetFiles(Environment.CurrentDirectory, "*.pdf", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        if (Path.GetFileName(pdfFile) != FacturaNamePDF && File.GetCreationTime(pdfFile).AddMinutes(30) <= DateTime.Now)
                            File.Delete(pdfFile);
                    }
                    catch { }
                }
                Process.Start(Path.Combine(Environment.CurrentDirectory, FacturaNamePDF));
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al mostrar el pdf", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ImprimirFactura(string FacturaNamePDF)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                Verb = "print",
                FileName = FacturaNamePDF
            };
            p.Start();
        }
    }
}
