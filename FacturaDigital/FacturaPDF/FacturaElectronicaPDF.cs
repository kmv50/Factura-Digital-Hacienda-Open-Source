using DataModel;
using DataModel.EF;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace FacturaDigital.FacturaPDF
{
    public class PDFFooter : PdfPageEventHelper
    {
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            PdfPTable tabFot = new PdfPTable(new float[] { 1F });            
            tabFot.SpacingAfter = 10F;
            PdfPCell cell;
            tabFot.TotalWidth = 500f;
            BaseFont font = BaseFont.CreateFont("c:\\windows\\fonts\\calibri.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            cell = new PdfPCell(new Phrase("Autorizada mediante resolución Nº DGT-R-48-2016 del 7 de octubre de 2016", new Font(font, 10)));
            cell.Border = Rectangle.NO_BORDER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;            
            tabFot.AddCell(cell);
            float yPosition = 20;
            float xPosition = 35;
            tabFot.WriteSelectedRows(0, -1, xPosition, yPosition, writer.DirectContent);
        }
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
    }

    public static class PdfContentByte_Extencion
    {
        public static void Linea(this PdfContentByte PDF, string txt, int x, int y, bool bold = false, int align = PdfContentByte.ALIGN_LEFT, int fontsize = 10)
        {
            if (bold)
            {
                PDF.SaveState();
                PDF.SetCharacterSpacing(0.5f);
                PDF.SetRGBColorFill(66, 00, 00);
                PDF.SetLineWidth((float)0.3);
                PDF.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE);
            }

            PDF.ShowTextAligned(align, txt, x, y, 0);

            if (bold)
            {
                PDF.RestoreState();
            }
        }


        public static void Parrafo(this PdfContentByte PDF, string txt, int x, int y, int xMAx = 0, int yMAx = 100)
        {
            //calibri
            PDF.SaveState();
            ColumnText ct = new ColumnText(PDF);
            BaseFont font = BaseFont.CreateFont("c:\\windows\\fonts\\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Phrase parrafo = new Phrase(new Chunk(txt, new Font(font, 10)));
            xMAx = xMAx == 0 ? x + 450 : xMAx;
            ct.SetSimpleColumn(parrafo, x, y, xMAx, yMAx, 12, Element.ALIGN_LEFT);
            ct.Go();
            PDF.RestoreState();
        }
    }

    public class FacturaElectronicaPDF : ILog
    {
        private readonly BaseFont f_cn = BaseFont.CreateFont("c:\\windows\\fonts\\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

        

        public string CrearFactura(Factura fac)
        {
            string NewPdfName = Guid.NewGuid().ToString()+".pdf";

            try
            {

                //La hoja de ancho mide 590
                using (System.IO.FileStream fs = new FileStream(NewPdfName, FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 25, 25, 30, 1);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.AddAuthor("Kevin Marin");
                    document.AddCreator("Factura Digital OS");
                    document.AddKeywords("Factura Digital OS");
                    document.AddSubject("PDF Factura Hacienda");
                    document.AddTitle("PDF Factura Hacienda");

                    document.Open();

                    writer.PageEvent = new PDFFooter();

                    PdfContentByte doc = writer.DirectContent;

                    string Logo = Directory.GetFiles(Environment.CurrentDirectory, "Logo.*",SearchOption.TopDirectoryOnly).FirstOrDefault();
                    if (!string.IsNullOrEmpty(Logo)) {
                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Logo);

                        if (img.Height > img.Width)
                        {
                            float percentage = 0.0f;
                            percentage = 100 / img.Height;
                            img.ScalePercent(percentage * 100);
                        }
                        else
                        {
                            float percentage = 0.0f;
                            percentage = 100 / img.Width;
                            img.ScalePercent(percentage * 100);
                        }

                        img.SetAbsolutePosition(30, 730);
                        doc.AddImage(img);
                    }

                    doc.BeginText();

                    doc.SetFontAndSize(f_cn, 10);


                    doc.Linea("Factura Electrónica V4.2", 350, 820, bold: true);
                    doc.Linea("Consecutivo", 350, 800);
                    doc.Linea(new ConsecutivoHacienda() {
                        CasaMatriz = fac.CasaMatriz,
                        NumeracionConsecutiva = fac.NumeroConsecutivo,
                        PuntoVenta = fac.PuntoVenta,
                        TipoDocumento = (Tipo_documento)fac.Id_TipoDocumento
                    }.ToString(), 420, 800, bold: true);

                    doc.Linea("Fecha", 350, 788);
                    doc.Linea(DateTime.Now.ToString(), 420, 788);

                    doc.Linea("Moneda", 350, 776);
                    doc.Linea("Colones", 420, 776);


                    int left_margin = 40;
                    int left_marginValues = 115;
                    int top_margin = 720;
                    doc.Linea("Datos Vendedor", left_margin, top_margin);
                    doc.Linea("Nombre:", left_margin, top_margin - 12);
                    doc.Linea(fac.Emisor_NombreComercial, left_marginValues, top_margin - 12);

                    doc.Linea("Identificación:", left_margin, top_margin - 24);
                    doc.Linea(fac.Emisor_Identificacion_Numero, left_marginValues, top_margin - 24);

                    doc.Linea("Teléfono:", left_margin, top_margin - 36);
                    doc.Linea(fac.Emisor_Telefono_Numero.ToString(), left_marginValues, top_margin - 36);

                    doc.Linea("Email:", left_margin, top_margin - 48);
                    doc.Linea(fac.Emisor_CorreoElectronico, left_marginValues, top_margin - 48);

                    doc.Linea("Dirección:", left_margin, top_margin - 60);
                    doc.Parrafo(FullAddress(fac), left_marginValues, top_margin - 50);

                    left_margin = 350;
                    int left_marginValuesSecondColumn  = 420;
                    doc.Linea("Datos Cliente", left_margin, top_margin);
                    doc.Linea("Nombre:", left_margin, top_margin - 12);
                    doc.Linea(string.IsNullOrEmpty(fac.Receptor_NombreComercial) ? fac.Receptor_Nombre : fac.Receptor_NombreComercial, left_marginValuesSecondColumn, top_margin - 12);

                    doc.Linea("Identificación:", left_margin, top_margin - 24);
                    doc.Linea(string.IsNullOrEmpty(fac.Receptor_Identificacion_Numero) ? "No se indica" : fac.Receptor_Identificacion_Numero, left_marginValuesSecondColumn, top_margin - 24);

                    doc.Linea("Teléfono:", left_margin, top_margin - 36);
                    doc.Linea(!fac.Receptor_Telefono_Numero.HasValue ? "No se indica" : fac.Receptor_Telefono_Numero.ToString(), left_marginValuesSecondColumn, top_margin - 36);

                    doc.Linea("Email:", left_margin, top_margin - 48);
                    doc.Linea(string.IsNullOrEmpty(fac.Receptor_CorreoElectronico) ? "No se indica" : fac.Receptor_CorreoElectronico, left_marginValuesSecondColumn, top_margin - 48);


                    left_margin = 40;
                    top_margin = 635;

                    doc.Linea("Condición Venta:", left_margin, top_margin);
                    doc.Linea(Utilides.GetCondicionVentaFullName(fac.CondicionVenta), left_marginValues, top_margin);

                    doc.Linea("Medio de Pago:", 40, top_margin - 12);
                    doc.Linea(Utilides.GetMedioDePagoFullName(fac.MedioPago), left_marginValues, top_margin - 12);

                    doc.Linea("Clave Hacienda:", left_margin, top_margin - 24);
                    doc.Linea(fac.Clave, left_marginValues, top_margin - 24);                   

                    left_margin = 40;
                    top_margin = 590;
                    doc.Linea("Detalle:", left_margin, top_margin);
                    doc.Parrafo("No se indica", left_marginValues, 610);


                    doc.EndText();
                    doc.SetLineWidth(0f);
                    doc.MoveTo(40, 570);
                    doc.LineTo(560, 570);
                    doc.Stroke();
                    doc.BeginText();

                    int lastwriteposition = 100;

  
                    top_margin = 550;
                    left_margin = 40;
                    doc.Linea("Cod", left_margin, top_margin);
                    doc.Linea("Description", left_margin + 30, top_margin);
                    doc.Linea("Cant", left_margin + 280, top_margin);
                    doc.Linea("Precio", left_margin + 310, top_margin);
                    doc.Linea("Impuesto", left_margin + 370, top_margin);
                    doc.Linea("Descuento", left_margin + 425, top_margin);
                    doc.Linea("Total", left_margin + 480, top_margin);

                    top_margin = 538;

                    decimal ImpuestoMonto = 0;
                    decimal DescuentoMonto = 0;
                    foreach (Factura_Detalle Item in fac.Factura_Detalle)
                    {
                        
                        ImpuestoMonto = Item.Impuesto_Monto ?? 0;
                        DescuentoMonto = Item.Monto_Descuento ?? 0;
                        doc.Linea(Item.Codigo, left_margin, top_margin);
                        doc.Linea(Item.ProductoServicio, left_margin + 30, top_margin);
                        doc.Linea(Item.Cantidad.ToString(), left_margin + 280, top_margin);
                        doc.Linea(Item.PrecioUnitario.ToString("₡#0.00"), left_margin + 310, top_margin);
                        doc.Linea(ImpuestoMonto.ToString("₡#0.00"), left_margin + 370, top_margin);
                        doc.Linea(DescuentoMonto.ToString("₡#0.00"), left_margin + 425, top_margin);
                        doc.Linea(Item.Monto_Total_Linea.ToString("₡#0.00"), left_margin + 480, top_margin);

                        top_margin -= 12;

                        if (top_margin <= lastwriteposition)
                        {
                            doc.EndText();
                            document.NewPage();
                            doc.BeginText();
                            top_margin = 780;
                        }
                    }

                    top_margin -= 80;
                    left_margin = 350;

                    writeText(doc, "SubTotal", left_margin, top_margin);
                    writeText(doc, "Descuento", left_margin, top_margin - 12);
                    writeText(doc, "Impuesto", left_margin, top_margin - 24);
                    writeText(doc, "Total", left_margin, top_margin - 48);
                    // Move right to write out the values
                    left_margin = 540;
                    // Write out the invoice currency and values in regular text

                    left_margin = 535;
                    doc.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, fac.TotalVenta.ToString("₡#0.00"), left_margin, top_margin, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, (fac.TotalDescuentos ?? 0 ).ToString("₡#0.00"), left_margin, top_margin - 12, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, (fac.TotalImpuesto ?? 0).ToString("₡#0.00"), left_margin, top_margin - 24, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, fac.TotalComprobante.ToString("₡#0.00"), left_margin, top_margin - 48, 0);

                    // End the writing of text
                    doc.EndText();

                    // Close the document, the writer and the filestream!
                    document.Close();
                    writer.Close();
                    fs.Close();


                }
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                MessageBox.Show("Error al crear el PDF", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }          
            return NewPdfName;
        }

        public void MostrarFactura(string FacturaNamePDF) {

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

        public void ImprimirFactura(string FacturaNamePDF) {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                Verb = "print",
                FileName = FacturaNamePDF 
            };
            p.Start();
        }

        private string FullAddress(Factura fac)
        {
            string Address = string.Empty;
            try
            {

                using (db_FacturaDigital db = new db_FacturaDigital())
                {                    
                  Ubicacion ub =  db.Ubicaciones.FirstOrDefault(q => 
                  q.Id_Barrio == fac.Emisor_Ubicacion_Barrio.Value
                  && q.Id_Provincia == fac.Emisor_Ubicacion_Provincia
                  && q.Id_Canton == fac.Emisor_Ubicacion_Canton
                  && q.Id_Distrito == fac.Emisor_Ubicacion_Distrito);

                  Address = ub.Provincia + " " + ub.Canton + " " + ub.Distrito + " " + ub.Barrio + " " + fac.Receptor_Ubicacion_OtrasSenas;
                }
            }catch(Exception ex)
            {
                this.LogError(ex);
            }

            return Address;
        }


        private void writeText(PdfContentByte cb, string Text, int X, int Y)
        {
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Text, X, Y, 0);
        }
    }
}
