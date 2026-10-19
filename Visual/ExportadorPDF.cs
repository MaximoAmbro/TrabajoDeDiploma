using System;
using System.Data;
using System.IO;
using System.Linq;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Negocio;

namespace Visual
{
    // Anexo A02: exportación a PDF con iText, por código, sin impresora virtual. Está en Visual
    // porque iText es una librería de presentación: Negocio solo conoce IExportador. No sabe nada
    // de ningún reporte en particular: lee encabezados, formatos y fila de totales de la tabla.
    //
    // Cinco zonas: encabezado, bloque de filtros, tabla, lectura y pie (fecha de emisión,
    // «Página N de M» y QR de verificación). El Document se crea sin volcado inmediato para poder
    // escribir el pie en cada página cuando ya se conoce el total.
    public sealed class ExportadorPDF : IExportador
    {
        private static readonly DeviceRgb Violeta = new DeviceRgb(116, 86, 174);
        private static readonly DeviceRgb Sombra = new DeviceRgb(231, 231, 231);
        private static readonly DeviceRgb FondoTotales = new DeviceRgb(236, 231, 246);
        private static readonly DeviceRgb FondoFiltros = new DeviceRgb(245, 245, 245);
        private static readonly DeviceRgb Gris = new DeviceRgb(100, 100, 100);

        private const float Margen = 40f;
        private const float LadoQR = 46f;

        private PdfFont normal;
        private PdfFont negrita;

        public string Exportar(Reporte r, DataTable datos, string ruta)
        {
            if (r == null)
            {
                throw new ArgumentNullException(nameof(r));
            }
            string carpeta = System.IO.Path.GetDirectoryName(ruta);
            if (!string.IsNullOrEmpty(carpeta) && !Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            DateTime emision = DateTime.Now;
            var pdf = new PdfDocument(new PdfWriter(ruta));
            var doc = new Document(pdf, PageSize.A4, false);
            doc.SetMargins(50, Margen, 80, Margen);
            CargarFuentes();

            EscribirEncabezado(doc, r);
            EscribirFiltros(doc, r);
            EscribirTabla(doc, datos, r.Encabezados());
            EscribirLectura(doc, r.Lectura(datos));
            EscribirPie(doc, pdf, r, emision);

            doc.Close();   // iText escribe el archivo en disco
            return ruta;
        }

        // La tipografía de la aplicación (Nirmala UI) si está instalada; si no, Helvetica.
        private void CargarFuentes()
        {
            string fuentes = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            string regular = System.IO.Path.Combine(fuentes, "Nirmala.ttf");
            string bold = System.IO.Path.Combine(fuentes, "NirmalaB.ttf");
            if (File.Exists(regular) && File.Exists(bold))
            {
                normal = PdfFontFactory.CreateFont(regular, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
                negrita = PdfFontFactory.CreateFont(bold, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            }
            else
            {
                normal = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                negrita = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            }
        }

        // No hay archivo de logotipo: la marca va como texto, en el violeta de la aplicación.
        private void EscribirEncabezado(Document doc, Reporte r)
        {
            doc.Add(new Paragraph("EventAura").SetFont(negrita).SetFontSize(22).SetFontColor(Violeta)
                .SetMarginBottom(0));
            doc.Add(new Paragraph($"{r.Titulo}  ({r.Codigo})").SetFont(negrita).SetFontSize(15).SetMarginTop(0)
                .SetMarginBottom(0));
            doc.Add(new Paragraph($"Período: {r.Desde:dd/MM/yyyy} al {r.Hasta:dd/MM/yyyy}")
                .SetFont(normal).SetFontSize(9).SetFontColor(Gris));
            var linea = new SolidLine(1.2f);
            linea.SetColor(Violeta);
            doc.Add(new LineSeparator(linea).SetMarginBottom(8));
        }

        private void EscribirFiltros(Document doc, Reporte r)
        {
            var bloque = new Div().SetBackgroundColor(FondoFiltros).SetPadding(6).SetMarginBottom(12);
            bloque.Add(new Paragraph().Add(new Text("Filtros aplicados: ").SetFont(negrita))
                .Add(new Text(r.Filtros()).SetFont(normal)).SetFontSize(8.5f).SetMargin(0));
            bloque.Add(new Paragraph().Add(new Text("Solicitado por: ").SetFont(negrita))
                .Add(new Text(r.Solicitante ?? "—").SetFont(normal)).SetFontSize(8.5f).SetMargin(0));
            doc.Add(bloque);
        }

        // Encabezado sombreado, números a la derecha y fila de totales resaltada.
        private void EscribirTabla(Document doc, DataTable datos, string[] cab)
        {
            float[] anchos = datos.Columns.Cast<DataColumn>()
                .Select(c => Derecha(c) ? 1.4f : 3f)
                .ToArray();
            Table tabla = new Table(UnitValue.CreatePercentArray(anchos)).UseAllAvailableWidth();

            for (int i = 0; i < cab.Length; i++)
            {
                tabla.AddHeaderCell(new Cell()
                    .Add(new Paragraph(cab[i]).SetFont(negrita).SetFontSize(8.5f))
                    .SetBackgroundColor(Sombra).SetPaddingLeft(4).SetPaddingRight(4)
                    .SetTextAlignment(Derecha(datos.Columns[i]) ? TextAlignment.RIGHT : TextAlignment.LEFT));
            }

            bool conTotales = datos.ExtendedProperties.ContainsKey(Reporte.PropiedadFilaTotales);
            for (int f = 0; f < datos.Rows.Count; f++)
            {
                bool esTotal = conTotales && f == datos.Rows.Count - 1;
                for (int c = 0; c < datos.Columns.Count; c++)
                {
                    var celda = new Cell()
                        .Add(new Paragraph(Texto(datos.Rows[f][c], datos.Columns[c]))
                            .SetFont(esTotal ? negrita : normal).SetFontSize(8.5f))
                        .SetTextAlignment(Derecha(datos.Columns[c]) ? TextAlignment.RIGHT : TextAlignment.LEFT)
                        .SetPaddingLeft(4).SetPaddingRight(4);
                    if (esTotal)
                    {
                        celda.SetBackgroundColor(FondoTotales);
                    }
                    tabla.AddCell(celda);
                }
            }
            doc.Add(tabla);
        }

        private void EscribirLectura(Document doc, string texto)
        {
            doc.Add(new Paragraph()
                .Add(new Text("Lectura. ").SetFont(negrita))
                .Add(new Text(texto).SetFont(normal))
                .SetFontSize(9.5f).SetMarginTop(14).SetTextAlignment(TextAlignment.JUSTIFIED));
        }

        // En cada página: emisión, «Página N de M» y el QR. Se crea una imagen nueva por página:
        // agregar la misma instancia de Image en todas no deja una copia en cada una.
        private void EscribirPie(Document doc, PdfDocument pdf, Reporte r, DateTime emision)
        {
            string verificacion = $"EVAU|{r.Codigo}|{r.Desde:yyyyMMdd}-{r.Hasta:yyyyMMdd}|" +
                                  $"{emision:yyyyMMddHHmmss}|{r.Solicitante}";
            byte[] qr = GeneradorQR.Instance.Imagen(verificacion);
            int total = pdf.GetNumberOfPages();
            for (int i = 1; i <= total; i++)
            {
                float ancho = pdf.GetPage(i).GetPageSize().GetWidth();
                doc.ShowTextAligned(
                    new Paragraph($"Emitido el {emision:dd/MM/yyyy HH:mm}   ·   Página {i} de {total}   ·   {r.NombreArchivo}.pdf")
                        .SetFont(normal).SetFontSize(7.5f).SetFontColor(Gris),
                    Margen, 30, i, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);
                Image imagen = new Image(ImageDataFactory.Create(qr)).ScaleAbsolute(LadoQR, LadoQR);
                imagen.SetFixedPosition(i, ancho - Margen - LadoQR, 18);
                doc.Add(imagen);
            }
        }

        private static bool Derecha(DataColumn columna)
        {
            return columna.ExtendedProperties.ContainsKey(Reporte.PropiedadFormato)
                || columna.ExtendedProperties.ContainsKey(Reporte.PropiedadDerecha);
        }

        private static string Texto(object valor, DataColumn columna)
        {
            if (valor == null || valor == DBNull.Value)
            {
                return "—";
            }
            string formato = columna.ExtendedProperties[Reporte.PropiedadFormato] as string;
            IFormattable formateable = valor as IFormattable;
            return formato != null && formateable != null
                ? formateable.ToString(formato, Reporte.Cultura)
                : Convert.ToString(valor, Reporte.Cultura);
        }
    }
}
