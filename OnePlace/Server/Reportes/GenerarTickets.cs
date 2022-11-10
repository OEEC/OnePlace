using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Globalization;
using QRCodeEncoderLibrary;
using BarcodeLib;
using Microsoft.AspNetCore.Hosting;
using iTextSharp.text.html;
using OnePlace.Shared.DTOs;

namespace OnePlace.Server.Reportes
{
    public class GenerarTickets
    {
        private static PdfPCell ImageCell(string path, float scale, int align)
        {
            iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance(path);
            return ImageToCell(scale, align, instance);
        }
        private static PdfPCell LogoCell(string path, int px)
        {
            iTextSharp.text.Image instance = iTextSharp.text.Image.GetInstance(path);

            float width = instance.Width;
            float height = instance.Height;
            float targetwidth = 200;// 68antes
            float scale = (targetwidth * px) / width;
            return ImageToCell(scale, 0, instance);
        }
        private static PdfPCell ImageToCell(float scale, int align, iTextSharp.text.Image image)
        {
            image.ScalePercent(scale);
            return new PdfPCell(image)
            {
                BorderColor = BaseColor.White,
                VerticalAlignment = 5,
                HorizontalAlignment = align,
                PaddingBottom = 0f,
                PaddingTop = 0f
            };
        }

        #region SinUsarPuedeDescomentar

        //public class PdfPageEvents : PdfPageEventHelper
        //{
        //    [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        //    private List<Equipo> datosparaeldocumento;
        //    public override void OnEndPage(PdfWriter writer, Document document)
        //    {
        //        PdfPTable table = new PdfPTable(1);
        //        Phrase phrase = null;
        //        PdfPCell cell = null;
        //        BaseColor color = new BaseColor(0x15, 0x7b, 0xff);
        //        BaseColor fontColor = new BaseColor(0xa9, 0xa9, 0xa9);
        //        BaseColor color3 = new BaseColor(220, 220, 220);
        //        BaseColor bLACK = BaseColor.Black;
        //        int px = 40;

        //        table = new PdfPTable(2)
        //        {
        //            DefaultCell = { Border = 0 },
        //            TotalWidth = (document.PageSize.Width - document.LeftMargin) - document.RightMargin,
        //            LockedWidth = true
        //        };
        //        float[] relativeWidths = new float[] { 0.15f, 0.85f };
        //        table.SetWidths(relativeWidths);

        //        //alternativa de usar Path.Combine(environment.WebRootPath, "logo_simsa.png");   
        //        string logo = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\ImgServer\gotasimsa.png"}";
        //        if (File.Exists(logo))
        //        {
        //            table.AddCell(LogoCell(logo, px));
        //        }
        //        else
        //        {
        //            table.AddCell(ImageCell(logo, 8f, 1));
        //        }

        //        PdfPTable table2 = new PdfPTable(4)
        //        {
        //            DefaultCell = { Border = 0 },
        //            TotalWidth = table.TotalWidth * 0.80f,
        //            LockedWidth = true
        //        };
        //        table2.SetWidths(new float[] { 0.25f, 0.25f, 0.25f, 0.25f });

        //        phrase = new Phrase();
        //        cell = this.PhraseCell(phrase, 8);
        //        cell.VerticalAlignment = 4;
        //        cell.Colspan = 4;
        //        table2.AddCell(cell);
        //        table2.AddCell(cell);
        //        table.AddCell(table2);
        //        table.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height, writer.DirectContent);
        //    }
        //    private PdfPCell PhraseCell(Phrase phrase, int align) =>
        //        new PdfPCell(phrase)
        //        {
        //            BorderColor = BaseColor.White,
        //            VerticalAlignment = 4,
        //            HorizontalAlignment = align,
        //            PaddingBottom = 2f,
        //            PaddingTop = 0f
        //        };
        //    public List<Equipo> _equipo
        //    {
        //        [CompilerGenerated]
        //        get =>
        //            this.datosparaeldocumento;
        //        [CompilerGenerated]
        //        set =>
        //            datosparaeldocumento = value;
        //    }
        //}

        #endregion SinUsarPuedeDescomentar
        private static PdfPCell PhraseCell(Phrase phrase, int align) =>
             new PdfPCell(phrase)
             {
                 BorderColor = BaseColor.White,
                 VerticalAlignment = 4,
                 HorizontalAlignment = align,
                 PaddingBottom = 2f,
                 PaddingTop = 0f
             };
        public static void Detalle(ref PdfPTable table, BaseColor FontColor, string Value, int Align, float Size = 8f, int Tipo = 0, int ColSpan = 1, int VerticalAlignment = 4)
        {
            PdfPCell cell = PhraseCell(new Phrase(Value, FontFactory.GetFont("Arial", Size, Tipo, FontColor)), Align);
            if (ColSpan > 1)
            {
                cell.Colspan = ColSpan;
            }
            cell.VerticalAlignment = VerticalAlignment;
            //Quitamos el Borde de cada celda de la Tabla
            cell.Border = Rectangle.NO_BORDER;
            table.AddCell(cell);
        }
        public static void TitDetalle(ref PdfPTable table, BaseColor FontColor, BaseColor BackColor, BaseColor LineColor, string Title, int Align, float Size = 8f, int Tipo = 0, int ColSpan = 1, int VerticalAlignment = 4)
        {
            PdfPCell cell = PhraseCell(new Phrase(Title, FontFactory.GetFont("Arial", Size, Tipo, FontColor)), Align);
            cell.BorderColorBottom = LineColor;
            cell.BorderColorLeft = BackColor;
            cell.BorderColorRight = BackColor;
            cell.BorderColorTop = BackColor;
            cell.BorderWidthBottom = 0.4f;
            cell.BackgroundColor = BackColor;
            if (ColSpan > 1)
            {
                cell.Colspan = ColSpan;
            }
            cell.VerticalAlignment = VerticalAlignment;
            table.AddCell(cell);
        }

        #region TarjetadeCumple
        public static byte[] TarjetadeCumple(EmpleadoPersonaDTO model)
        {
            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            MemoryStream PDFData = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, PDFData);
            Rectangle pageSize = pdfWriter.PageSize;
            pdfDoc.Open();
                 
            //BaseColor ColorSimsa = WebColors.GetRgbColor("#080706");
            //var titleFont = FontFactory.GetFont("Pacific", 20, ColorSimsa);
            
            //Top Heading
            Chunk chunk = new Chunk("Te deseamos lo mejor, que te la pases genial!!", FontFactory.GetFont("Arial", 20, Font.BOLDITALIC, BaseColor.Magenta));
            pdfDoc.Add(chunk);

            //Horizontal Line
            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.Black, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            //Table
            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            //0=Left, 1=Centre, 2=Right
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            //Cell no 1
            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            string imgCombine = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\ImgServer\cumplecake.jpg"}";
            Image image = Image.GetInstance(imgCombine);           
            //image.ScaleAbsolute(200, 150);
            cell.AddElement(image);
            table.AddCell(cell);

            //Cell no 2
            //chunk = new Chunk("Name: Mrs. Salma Mukherji,\nAddress: Latham Village, Latham, New York, US, \nOccupation: Nurse, \nAge: 35 years", FontFactory.GetFont("Arial", 15, Font.NORMAL, BaseColor.Pink));
            //cell = new PdfPCell();
            //cell.Border = 0;
            //cell.AddElement(chunk);
            //table.AddCell(cell);

            //Add table to document
            pdfDoc.Add(table);

            //Horizontal Line
            //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.Black, Element.ALIGN_LEFT, 1)));
            //pdfDoc.Add(line);

            ////Table
            //table = new PdfPTable(5);
            //table.WidthPercentage = 100;
            //table.HorizontalAlignment = 0;
            //table.SpacingBefore = 20f;
            //table.SpacingAfter = 30f;

            ////Cell
            //cell = new PdfPCell();
            //chunk = new Chunk("This Month's Transactions of your Credit Card");
            //cell.AddElement(chunk);
            //cell.Colspan = 5;
            //cell.BackgroundColor = BaseColor.Pink;
            //table.AddCell(cell);

            //table.AddCell("S.No");
            //table.AddCell("NYC Junction");
            //table.AddCell("Item");
            //table.AddCell("Cost");
            //table.AddCell("Date");

            //table.AddCell("1");
            //table.AddCell("David Food Store");
            //table.AddCell("Fruits & Vegetables");
            //table.AddCell("$100.00");
            //table.AddCell("June 1");

            //table.AddCell("2");
            //table.AddCell("Child Store");
            //table.AddCell("Diaper Pack");
            //table.AddCell("$6.00");
            //table.AddCell("June 9");

            //table.AddCell("3");
            //table.AddCell("Punjabi Restaurant");
            //table.AddCell("Dinner");
            //table.AddCell("$29.00");
            //table.AddCell("June 15");

            //table.AddCell("4");
            //table.AddCell("Wallmart Albany");
            //table.AddCell("Grocery");
            //table.AddCell("$299.50");
            //table.AddCell("June 25");

            //table.AddCell("5");
            //table.AddCell("Singh Drugs");
            //table.AddCell("Back Pain Tablets");
            //table.AddCell("$14.99");
            //table.AddCell("June 28");

            //pdfDoc.Add(table);

            var nombre = "";
            if (model != null)
            {
                nombre = model.Persona.Nombre + " " + model.Persona.ApePat;
            } 

            var textpink  = FontFactory.GetFont("Arial", 15, Font.NORMAL, BaseColor.Pink);
            var descriptionChunk = new Chunk("¡¡Feliz Cumpleaños "+nombre+"!!"+"\n\n"+model.ProximoCumple.ToString("dd-MM-yyyy")+"\n\n“Trabajar contigo es un aprendizaje constante. Gracias por darnos tu apoyo y esfuerzo. Esta empresa no sería la misma sin ti.”\n\nAtte:\nGrupo Simsa.", textpink);
            Paragraph para = new Paragraph();
            para.Add(descriptionChunk);
            pdfDoc.Add(para);               

            #region Footer
            var time = FontFactory.GetFont(FontFactory.HELVETICA, 11f, Font.NORMAL);

            //var leftCol = new Paragraph("Mukesh Salaria\n" + "Software Engineer", time);
            var rightCol = new Paragraph("\n\n", time);//hacer salto de linea sino los texto quedar arriba de la linea negra
            var phone = new Paragraph("Responsable de Asignación", time);
            var address = new Paragraph("Firma de Conformidad del Usuario", time);
            var fax = new Paragraph("Firma del Jefe de Depto.", time);

            //leftCol.Alignment = Element.ALIGN_LEFT;
            rightCol.Alignment = Element.ALIGN_RIGHT;
            fax.Alignment = Element.ALIGN_RIGHT;
            phone.Alignment = Element.ALIGN_LEFT;
            address.Alignment = Element.ALIGN_CENTER;

            var footerTbl = new PdfPTable(3) { TotalWidth = 520f, HorizontalAlignment = Element.ALIGN_CENTER, LockedWidth = true };
            float[] widths = { 180f, 290f, 50f }; //original 150f, 220f, 150f
            footerTbl.SetWidths(widths);
            var footerCell1 = new PdfPCell(new Phrase(""));
            var footerCell2 = new PdfPCell();
            var footerCell3 = new PdfPCell(rightCol);
            var sep = new PdfPCell();

            var footerCell4 = new PdfPCell();
            string imgCombine2 = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\ImgServer\logo_simsa.png"}";
            Image image2 = Image.GetInstance(imgCombine2);
            image2.ScaleAbsolute(45, 45);
            footerCell4.AddElement(image2);         

            var footerCell5 = new PdfPCell();

            var footerCell6 = new PdfPCell();
            string imageString = model.CodigoQR.Substring(24);//se le quita data: image/png; base64, para que sea valida la conversion nuevamente a bytes
            byte[] imageBytes = Convert.FromBase64String(imageString);
            Image image3 = Image.GetInstance(imageBytes);
            image3.ScaleAbsolute(45, 45);
            footerCell6.AddElement(image3);

            footerCell1.Border = 0;
            footerCell2.Border = 0;
            footerCell3.Border = 0;
            footerCell4.Border = 0;
            footerCell5.Border = 0;
            footerCell6.Border = 0;
            footerCell6.HorizontalAlignment = Element.ALIGN_RIGHT;
            sep.Border = 0;
            sep.FixedHeight = 10f;
            footerCell3.HorizontalAlignment = Element.ALIGN_RIGHT;
            footerCell6.PaddingLeft = 0;
            sep.Colspan = 3;

            footerTbl.AddCell(footerCell1);
            footerTbl.AddCell(footerCell2);
            footerTbl.AddCell(footerCell3);
            footerTbl.AddCell(sep);
            footerTbl.AddCell(footerCell4);
            footerTbl.AddCell(footerCell5);
            footerTbl.AddCell(footerCell6);
            footerTbl.WriteSelectedRows(0, -1, 40, 80, pdfWriter.DirectContent);

            #endregion Footer

            PdfContentByte cb = new PdfContentByte(pdfWriter);

            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1250, true);
            cb = new PdfContentByte(pdfWriter);
            cb = pdfWriter.DirectContent;
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetLeft(10), 10);//original 120 ,20
            //cb.ShowText("Nota: El usuario es el único responsable del buen uso del equipo proporcionado,todo cambio debe ser notificado al Depto.de Sistemas.");
            cb.EndText();

            //Mueva el puntero y dibuje una línea para separar la sección de pie de página del resto de la página linea negra
            cb.MoveTo(40, pdfDoc.PageSize.GetBottom(50));
            cb.LineTo(pdfDoc.PageSize.Width - 40, pdfDoc.PageSize.GetBottom(50));
            cb.Stroke();

            pdfWriter.CloseStream = false;
            pdfDoc.Close();        

            byte[] arch = PDFData.ToArray();           
            PDFData.Close();
            return arch;
        }

        #endregion TarjetadeCumple

        #region CertificadoRojo
        public static byte[] Certificado(EmpleadoPersonaDTO model)
        {
            //pdf que se carga con todo e imagen           
            string pathPDF = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\ImgServer\certificadorojo.pdf"}";

            //archivo pdf nuevo con texto
            //string pathnewPDF = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\ImgServer\Pagarelimpiocontexto.pdf"}";

            //Objeto para leer el pdf original
            PdfReader reader = new PdfReader(pathPDF);

            //es para escribir el archivo en memoria y retronarlo en bytes
            MemoryStream PDFData = new MemoryStream();

            //el filestream es para escribir el archivo en la segunda ruta
            //FileStream fs = new FileStream(pathnewPDF, FileMode.Create, FileAccess.Write); 

            //crea el objeto PdfStamper para escribir para obtener las páginas del lector
            PdfStamper stamper = new PdfStamper(reader, PDFData);

            //El contenido del pdf, aqui se hace la escritura del contenido sobre el contenido origina, hara que el texto este encima de la imagen getover
            PdfContentByte canvas = stamper.GetOverContent(1);

            //Propiedades de nuestra fuente a insertar
            //BaseFont bf = BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            //obtener una fuente perzonalizada o de internet
            //string pathFonts = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FontsServer\Shelly.ttf"}";
            string pathFonts = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FontsServer\PlayfairDisplay-Italic.ttf"}";
            var bf = FontFactory.GetFont(pathFonts, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 28, Font.NORMAL, BaseColor.Black);
            var bf2 = FontFactory.GetFont(pathFonts, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 18, Font.NORMAL, BaseColor.Black);
            var bf3 = FontFactory.GetFont(pathFonts, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 14, Font.NORMAL, BaseColor.Black);

            ColumnText.ShowTextAligned(
                canvas,
                Element.ALIGN_RIGHT,
                new Phrase(model.Persona.Nombre + " " + model.Persona.ApePat + " " + model.Persona.ApeMat, new Font(bf)),
                650, 285, 0
            );

            ColumnText.ShowTextAligned(
              canvas,
              Element.ALIGN_RIGHT,
              new Phrase(model.Curso.Nombre, new Font(bf2)),
              540, 195, 0
            );

            ColumnText.ShowTextAligned(
             canvas,
             Element.ALIGN_RIGHT,
             new Phrase("Torreón Coahuila " + "a" + " " + DateTime.Now.ToString("dd") + " " + "de" + " " + DateTime.Now.ToString("MMMM") + " " + "del" + " " + DateTime.Now.ToString("yyyy"), new Font(bf3)),
             580, 150, 0
            );

            stamper.Close();

            byte[] arch = PDFData.ToArray();
            PDFData.Close();
            return arch;
        }

        #endregion

        #region CertificadoGris
        //public static byte[] Certificado(EmpleadoPersonaDTO model)
        //{
        //    //pdf que se carga con todo e imagen           
        //    string pathPDF = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\ImgServer\certificadogris.pdf"}";

        //    //archivo pdf nuevo con texto
        //    //string pathnewPDF = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\ImgServer\Pagarelimpiocontexto.pdf"}";

        //    //Objeto para leer el pdf original
        //    PdfReader reader = new PdfReader(pathPDF);

        //    //es para escribir el archivo en memoria y retronarlo en bytes
        //    MemoryStream PDFData = new MemoryStream();

        //    //el filestream es para escribir el archivo en la segunda ruta
        //    //FileStream fs = new FileStream(pathnewPDF, FileMode.Create, FileAccess.Write); 

        //    //crea el objeto PdfStamper para escribir para obtener las páginas del lector
        //    PdfStamper stamper = new PdfStamper(reader, PDFData);

        //    //El contenido del pdf, aqui se hace la escritura del contenido sobre el contenido origina, hara que el texto este encima de la imagen getover
        //    PdfContentByte canvas = stamper.GetOverContent(1);

        //    //Propiedades de nuestra fuente a insertar
        //    //BaseFont bf = BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        //    //obtener una fuente perzonalizada o de internet
        //    //string pathFonts = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FontsServer\Shelly.ttf"}";
        //    string pathFonts = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\FontsServer\PlayfairDisplay-Italic.ttf"}";
        //    var bf = FontFactory.GetFont(pathFonts, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 28, Font.NORMAL, BaseColor.Black);
        //    var bf2 = FontFactory.GetFont(pathFonts, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 18, Font.NORMAL, BaseColor.Black);
        //    var bf3 = FontFactory.GetFont(pathFonts, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, 14, Font.NORMAL, BaseColor.Black);

        //    ColumnText.ShowTextAligned(
        //        canvas,
        //        Element.ALIGN_RIGHT,
        //        new Phrase(model.Persona.Nombre + " " + model.Persona.ApePat + " " + model.Persona.ApeMat, new Font(bf)),
        //        660, 210, 0
        //    );

        //    ColumnText.ShowTextAligned(
        //      canvas,
        //      Element.ALIGN_RIGHT,
        //      new Phrase(model.Curso.Nombre, new Font(bf2)),
        //      550, 130, 0
        //    );

        //    ColumnText.ShowTextAligned(
        //     canvas,
        //     Element.ALIGN_RIGHT,
        //     new Phrase("Torreón Coahuila " + "a" + " " + DateTime.Now.ToString("dd") + " " + "de" + " " + DateTime.Now.ToString("MMMM") + " " + "del" + " " + DateTime.Now.ToString("yyyy"), new Font(bf3)),
        //     615, 95, 0
        //    );

        //    stamper.Close();

        //    byte[] arch = PDFData.ToArray();
        //    PDFData.Close();
        //    return arch;
        //}

        #endregion
    }
}
