using DataModel;
using DataModel.EF;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace FacturaDigital.FacturaPDF
{
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
            PDF.SaveState();
            ColumnText ct = new ColumnText(PDF);
            BaseFont font = BaseFont.CreateFont("c:\\windows\\fonts\\calibri.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            Phrase parrafo = new Phrase(new Chunk(txt, new Font(font, 10)));
            xMAx = xMAx == 0 ? x + 450 : xMAx;
            ct.SetSimpleColumn(parrafo, x, y, xMAx, yMAx, 12, Element.ALIGN_LEFT);
            ct.Go();
            PDF.RestoreState();
        }
    }

    public class FacturaElectronicaPDF : ILog
    {
        private readonly BaseFont f_cn = BaseFont.CreateFont("c:\\windows\\fonts\\calibri.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);



        public void CrearFactura(Factura fac)
        {

            try
            {


                using (System.IO.FileStream fs = new FileStream("test.pdf", FileMode.Create))
                {
                    Document document = new Document(PageSize.A4, 25, 25, 30, 1);
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);

                    document.AddAuthor("Kevin Marin");
                    document.AddCreator("Factura Digital OS");
                    document.AddKeywords("Factura Digital OS");
                    document.AddSubject("PDF Factura Hacienda");
                    document.AddTitle("PDF Factura Hacienda");

                    document.Open();


                    PdfContentByte doc = writer.DirectContent;
                    //  cb.AddTemplate(PdfFooter(cb, drPayee), 30, 1);

                    //iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(Server.MapPath("mbase_emc2.png"));
                    //png.ScaleAbsolute(200, 55);
                    //png.SetAbsolutePosition(40, 750);
                    //cb.AddImage(png);

                    // First we must activate writing
                    doc.BeginText();

                    doc.SetFontAndSize(f_cn, 10);

                    // First we write out the header information

                    // Start with the invoice type header
                    doc.Linea("Factura Electrónica V4.2", 350, 820, bold: true);
                    doc.Linea("Consecutivo", 350, 800);
                    doc.Linea("123456789123456789", 420, 800, bold: true);

                    doc.Linea("Fecha", 350, 788);
                    doc.Linea(DateTime.Now.ToString(), 420, 788);

                    doc.Linea("Moneda", 350, 776);
                    doc.Linea("Colones", 420, 776);


                    // Delivery address details
                    int left_margin = 40;
                    int left_marginValues = 110;
                    int top_margin = 750;
                    doc.Linea("Datos Vendedor", left_margin, top_margin);
                    doc.Linea("Nombre:", left_margin, top_margin - 12);
                    doc.Linea("Nombre:", left_marginValues, top_margin - 12);

                    doc.Linea("Identificación:", left_margin, top_margin - 24);
                    doc.Linea("Nombre:", left_marginValues, top_margin - 24);

                    doc.Linea("Teléfono:", left_margin, top_margin - 36);
                    doc.Linea("Nombre:", left_marginValues, top_margin - 36);

                    doc.Linea("Email:", left_margin, top_margin - 48);
                    doc.Linea("Nombre:", left_marginValues, top_margin - 48);

                    doc.Linea("Dirección:", left_margin, top_margin - 60);
                    doc.Parrafo("BMW (originally an initialism for Bayerische Motoren Werke in German, or Bavarian Motor Works in English) is a German multinational company which currently produces luxury automobiles and motorcycles, and also produced aircraft engines until 1945.", left_marginValues, top_margin - 50);

                    // Invoice address
                    left_margin = 350;
                    left_marginValues = 420;
                    doc.Linea("Datos Cliente", left_margin, top_margin);
                    doc.Linea("Nombre:", left_margin, top_margin - 12);
                    doc.Linea(string.IsNullOrEmpty(fac.Emisor_NombreComercial) ? fac.Emisor_Nombre : fac.Emisor_NombreComercial, left_marginValues, top_margin - 12);

                    doc.Linea("Identificación:", left_margin, top_margin - 24);
                    doc.Linea(string.IsNullOrEmpty(fac.Emisor_Identificacion_Numero) ? "No se indica" : fac.Emisor_Identificacion_Numero, left_marginValues, top_margin - 24);

                    doc.Linea("Teléfono:", left_margin, top_margin - 36);
                    doc.Linea(!fac.Emisor_Telefono_Numero.HasValue ? "No se indica" : fac.Emisor_Telefono_Numero.ToString(), left_marginValues, top_margin - 36);

                    doc.Linea("Email:", left_margin, top_margin - 48);
                    doc.Linea(string.IsNullOrEmpty(fac.Emisor_CorreoElectronico) ? "No se indica" : fac.Emisor_CorreoElectronico, left_marginValues, top_margin - 48);


                    // Write out invoice terms details
                    left_margin = 40;
                    top_margin = 620;
                    doc.Linea("Clave Hacienda", left_margin, top_margin);
                    doc.Linea("Dato1", left_margin, top_margin - 12);
                    doc.Linea("Condición Venta", left_margin + 200, top_margin);
                    doc.Linea("Dato1", left_margin + 200, top_margin - 12);
                    doc.Linea("Medio de Pago", left_margin + 350, top_margin);
                    doc.Linea("Dato1", left_margin + 350, top_margin - 12);
                    // Move down
                    left_margin = 40;
                    top_margin = 590;
                    doc.Linea("Detalle:", left_margin, top_margin);
                    doc.Parrafo("BMW (originally an initialism for Bayerische Motoren Werke in German, or Bavarian Motor Works in English) is a German multinational company which currently produces luxury automobiles and motorcycles, and also produced aircraft engines until 1945.", 110, 610);


                    // NOTE! You need to call the EndText() method before we can write graphics to the document!
                    doc.EndText();
                    // Separate the header from the rows with a line
                    // Draw a line by setting the line width and position
                    doc.SetLineWidth(0f);
                    doc.MoveTo(40, 570);
                    doc.LineTo(560, 570);
                    doc.Stroke();
                    // Don't forget to call the BeginText() method when done doing graphics!
                    doc.BeginText();

                    // Before we write the lines, it's good to assign a "last position to write"
                    // variable to validate against if we need to make a page break while outputting.
                    // Change it to 510 to write to test a page break; the fourth line on a new page
                    int lastwriteposition = 100;

                    // Loop thru the rows in the rows table
                    // Start by writing out the line headers
                    top_margin = 550;
                    left_margin = 40;
                    // Line headers
                    doc.Linea("Codigo", left_margin, top_margin);
                    doc.Linea("Description", left_margin + 70, top_margin);
                    doc.Linea("Cantida", left_margin + 300, top_margin);
                    doc.Linea("Precio", left_margin + 350, top_margin);
                    doc.Linea("Impuesto", left_margin + 400, top_margin);
                    doc.Linea("Descuento", left_margin + 450, top_margin);
                    doc.Linea("Total", left_margin + 500, top_margin);

                    // First item line position starts here
                    top_margin = 538;

                    decimal ImpuestoMonto = 0;
                    decimal DescuentoMonto = 0;
                    foreach (Factura_Detalle Item in fac.Factura_Detalle)
                    {
                        ImpuestoMonto = Item.Impuesto_Monto ?? 0;
                        DescuentoMonto = Item.Monto_Descuento ?? 0;
                        doc.Linea(Item.Codigo, left_margin, top_margin);
                        doc.Linea(Item.ProductoServicio, left_margin + 70, top_margin);
                        doc.Linea(Item.Cantidad.ToString(), left_margin + 300, top_margin);
                        doc.Linea(Item.PrecioUnitario.ToString("₡#.00"), left_margin + 350, top_margin);
                        doc.Linea(ImpuestoMonto.ToString("₡#.00"), left_margin + 400, top_margin);
                        doc.Linea(DescuentoMonto.ToString("₡#.00"), left_margin + 450, top_margin);
                        doc.Linea(Item.Monto_Total_Linea.ToString("₡#.00"), left_margin + 500, top_margin);

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

                    // First the headers
                    writeText(doc, "Invoice line totals", left_margin, top_margin);
                    writeText(doc, "Freight fee", left_margin, top_margin - 12);
                    writeText(doc, "VAT amount", left_margin, top_margin - 24);
                    writeText(doc, "Invoice grand total", left_margin, top_margin - 48);
                    // Move right to write out the values
                    left_margin = 540;
                    // Write out the invoice currency and values in regular text
                    string curr = "Total";
                    doc.ShowTextAligned(PdfContentByte.ALIGN_LEFT, curr, left_margin, top_margin, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_LEFT, curr, left_margin, top_margin - 12, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_LEFT, curr, left_margin, top_margin - 24, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_LEFT, curr, left_margin, top_margin - 48, 0);
                    left_margin = 535;
                    doc.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Total1", left_margin, top_margin, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Total1", left_margin, top_margin - 12, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Total1", left_margin, top_margin - 24, 0);
                    doc.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Total1", left_margin, top_margin - 48, 0);

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
            }
        }


        private void writeText(PdfContentByte cb, string Text, int X, int Y)
        {
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, Text, X, Y, 0);
        }
    }
}
