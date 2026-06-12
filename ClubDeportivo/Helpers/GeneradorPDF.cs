using System;
using System.Diagnostics;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ClubDeportivo.Helpers
{
    /// <summary>
    /// Genera documentos PDF de recibos usando PdfSharp.
    /// Los archivos se guardan en Documentos\ClubDeportivo\.
    /// </summary>
    public static class GeneradorPDF
    {
        // ── Colores del club ──────────────────────────────────────────
        private static readonly XColor ColorPrimario   = XColor.FromArgb(25,  80,  150);
        private static readonly XColor ColorSecundario = XColor.FromArgb(240, 242, 245);
        private static readonly XColor ColorTexto      = XColor.FromArgb(40,  40,  40);
        private static readonly XColor ColorGris       = XColor.FromArgb(120, 120, 120);

        // ── Fuentes ───────────────────────────────────────────────────
        private static readonly XFont FuenteTitulo   = new XFont("Arial", 18, XFontStyleEx.Bold);
        private static readonly XFont FuenteSubtitulo = new XFont("Arial", 11, XFontStyleEx.Regular);
        private static readonly XFont FuenteEtiqueta = new XFont("Arial",  9, XFontStyleEx.Bold);
        private static readonly XFont FuenteValor    = new XFont("Arial",  9, XFontStyleEx.Regular);
        private static readonly XFont FuentePie      = new XFont("Arial",  8, XFontStyleEx.Italic);
        private static readonly XFont FuenteNumero   = new XFont("Arial", 10, XFontStyleEx.Bold);

        // ─────────────────────────────────────────────────────────────
        //  RECIBO DE ALTA DE SOCIO
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Genera el comprobante de alta de socio y devuelve la ruta del PDF creado.
        /// </summary>
        public static string GenerarReciboAltaSocio(
            string nombreCompleto,
            string dni,
            string numeroSocio,
            string numeroCarnet,
            DateTime fechaAlta,
            decimal importeCuota,
            string medioPago)
        {
            string carpeta = ObtenerCarpeta("Altas");
            string archivo = Path.Combine(carpeta,
                $"AltaSocio_{numeroSocio}_{fechaAlta:yyyyMMdd_HHmmss}.pdf");

            using var doc = new PdfDocument();
            doc.Info.Title   = "Comprobante de Alta de Socio - Club Deportivo";
            doc.Info.Author  = "Club Deportivo";
            doc.Info.Creator = "Sistema Club Deportivo";

            var pag = doc.AddPage();
            pag.Width  = XUnit.FromMillimeter(210);
            pag.Height = XUnit.FromMillimeter(297);

            using var gfx = XGraphics.FromPdfPage(pag);

            double y = DibujarEncabezado(gfx, pag.Width, "COMPROBANTE DE ALTA DE SOCIO",
                $"Fecha: {fechaAlta:dd/MM/yyyy HH:mm}");

            y = DibujarSeccion(gfx, y, "DATOS DEL SOCIO", pag.Width, new[]
            {
                ("Nombre y Apellido:",  nombreCompleto),
                ("DNI:",                dni),
                ("Nº de Socio:",        numeroSocio),
                ("Nº de Carnet:",       numeroCarnet),
                ("Fecha de Alta:",      fechaAlta.ToString("dd/MM/yyyy")),
            });

            y = DibujarSeccion(gfx, y, "DATOS DEL PAGO", pag.Width, new[]
            {
                ("Cuota inicial:",      $"$ {importeCuota:N2}"),
                ("Medio de pago:",      medioPago),
            });

            DibujarPie(gfx, pag.Width, pag.Height,
                "Este comprobante acredita el alta como socio del Club Deportivo.");

            doc.Save(archivo);
            return archivo;
        }

        // ─────────────────────────────────────────────────────────────
        //  RECIBO DE COBRO DE CUOTA
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Genera el recibo de cobro de cuota y devuelve la ruta del PDF creado.
        /// </summary>
        public static string GenerarReciboCuota(
            string nombreCompleto,
            string numeroSocio,
            decimal importe,
            string medioPago,
            DateTime fechaPago,
            DateTime? nuevoVencimiento)
        {
            string carpeta = ObtenerCarpeta("Recibos");
            string archivo = Path.Combine(carpeta,
                $"ReciboCuota_{numeroSocio}_{fechaPago:yyyyMMdd_HHmmss}.pdf");

            using var doc = new PdfDocument();
            doc.Info.Title   = "Recibo de Cuota - Club Deportivo";
            doc.Info.Author  = "Club Deportivo";
            doc.Info.Creator = "Sistema Club Deportivo";

            var pag = doc.AddPage();
            pag.Width  = XUnit.FromMillimeter(210);
            pag.Height = XUnit.FromMillimeter(297);

            using var gfx = XGraphics.FromPdfPage(pag);

            double y = DibujarEncabezado(gfx, pag.Width, "RECIBO DE PAGO DE CUOTA",
                $"Fecha: {fechaPago:dd/MM/yyyy HH:mm}");

            y = DibujarSeccion(gfx, y, "DATOS DEL SOCIO", pag.Width, new[]
            {
                ("Nombre y Apellido:", nombreCompleto),
                ("Nº de Socio:",       numeroSocio),
            });

            y = DibujarSeccion(gfx, y, "DATOS DEL PAGO", pag.Width, new[]
            {
                ("Importe abonado:",   $"$ {importe:N2}"),
                ("Medio de pago:",     medioPago),
                ("Fecha de pago:",     fechaPago.ToString("dd/MM/yyyy HH:mm")),
                ("Nuevo vencimiento:", nuevoVencimiento.HasValue
                                         ? nuevoVencimiento.Value.ToString("dd/MM/yyyy")
                                         : "—"),
            });

            DibujarPie(gfx, pag.Width, pag.Height,
                "Conserve este recibo como comprobante de pago.");

            doc.Save(archivo);
            return archivo;
        }

        // ─────────────────────────────────────────────────────────────
        //  HELPERS DE DIBUJO
        // ─────────────────────────────────────────────────────────────

        /// Encabezado con fondo azul, nombre del club, título y fecha.
        /// Devuelve la Y donde termina.
        private static double DibujarEncabezado(XGraphics gfx, XUnit ancho,
            string titulo, string subtitulo)
        {
            double w = ancho.Point;

            // Franja azul superior
            gfx.DrawRectangle(new XSolidBrush(ColorPrimario),
                0, 0, w, 80);

            // Logo textual del club
            gfx.DrawString("CLUB DEPORTIVO", FuenteTitulo, XBrushes.White,
                new XRect(0, 14, w, 30), XStringFormats.TopCenter);
            gfx.DrawString(titulo, FuenteSubtitulo,
                new XSolidBrush(XColor.FromArgb(180, 210, 255)),
                new XRect(0, 46, w, 20), XStringFormats.TopCenter);

            // Línea separadora
            gfx.DrawLine(new XPen(XColor.FromArgb(200, 200, 200)), 40, 90, w - 40, 90);

            // Subtítulo (fecha)
            gfx.DrawString(subtitulo, FuenteValor, new XSolidBrush(ColorGris),
                new XRect(0, 94, w, 16), XStringFormats.TopCenter);

            return 118;
        }

        /// Sección con título y tabla de etiqueta–valor.
        /// Devuelve la Y donde termina la sección.
        private static double DibujarSeccion(XGraphics gfx, double y, string titulo,
            XUnit ancho, (string etiqueta, string valor)[] filas)
        {
            double w   = ancho.Point;
            double mg  = 48;            // margen horizontal
            double col = 180;           // ancho columna etiqueta
            double alt = 22;            // alto de cada fila

            // Título de sección
            gfx.DrawRectangle(new XSolidBrush(ColorSecundario), mg, y, w - mg * 2, 20);
            gfx.DrawString(titulo, FuenteEtiqueta, new XSolidBrush(ColorPrimario),
                new XRect(mg + 6, y + 3, w - mg * 2 - 12, 16), XStringFormats.TopLeft);
            y += 24;

            // Filas
            foreach (var (etq, val) in filas)
            {
                gfx.DrawString(etq, FuenteEtiqueta, new XSolidBrush(ColorTexto),
                    new XRect(mg, y, col, alt), XStringFormats.TopLeft);
                gfx.DrawString(val, FuenteValor, new XSolidBrush(ColorTexto),
                    new XRect(mg + col, y, w - mg * 2 - col, alt), XStringFormats.TopLeft);

                // Línea divisora sutil
                gfx.DrawLine(new XPen(XColor.FromArgb(220, 220, 220)),
                    mg, y + alt - 2, w - mg, y + alt - 2);

                y += alt;
            }

            return y + 12;
        }

        /// Pie de página centrado al fondo.
        private static void DibujarPie(XGraphics gfx, XUnit ancho, XUnit alto, string texto)
        {
            double w = ancho.Point;
            double h = alto.Point;

            gfx.DrawLine(new XPen(XColor.FromArgb(200, 200, 200)), 48, h - 45, w - 48, h - 45);
            gfx.DrawString(texto, FuentePie, new XSolidBrush(ColorGris),
                new XRect(0, h - 38, w, 14), XStringFormats.TopCenter);
            gfx.DrawString("Club Deportivo © " + DateTime.Now.Year,
                FuentePie, new XSolidBrush(ColorGris),
                new XRect(0, h - 22, w, 14), XStringFormats.TopCenter);
        }

        // ─────────────────────────────────────────────────────────────
        //  UTILIDADES DE ARCHIVO
        // ─────────────────────────────────────────────────────────────

        /// Devuelve (y crea si hace falta) la subcarpeta dentro de
        /// Documentos\ClubDeportivo\<subcarpeta>.
        private static string ObtenerCarpeta(string subcarpeta)
        {
            string base_ = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "ClubDeportivo", subcarpeta);
            Directory.CreateDirectory(base_);
            return base_;
        }

        /// Abre en el Explorador de Windows la carpeta donde se guardó el PDF.
        public static void AbrirCarpeta(string rutaArchivo)
        {
            string? carpeta = Path.GetDirectoryName(rutaArchivo);
            if (carpeta != null && Directory.Exists(carpeta))
                Process.Start("explorer.exe", carpeta);
        }
    }
}
