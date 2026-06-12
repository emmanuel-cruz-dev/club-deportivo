using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClubDeportivo.Helpers;

namespace ClubDeportivo.Forms
{
    /// <summary>
    /// Formulario de salida: muestra el recibo de pago de cuota
    /// y permite guardar el PDF o simplemente cerrarlo.
    /// </summary>
    public partial class FrmReciboSocio : Form
    {
        // ── Datos del recibo ──────────────────────────────────────────
        private readonly string _nombreCompleto;
        private readonly string _numeroSocio;
        private readonly decimal _importe;
        private readonly string _medioPago;
        private readonly DateTime _fechaPago;
        private readonly DateTime? _nuevoVencimiento;

        // ── Controles ────────────────────────────────────────────────
        private Panel panelEncabezado;
        private Label lblTitulo, lblSubtitulo;
        private Panel panelCuerpo;
        private Button btnGuardarPDF, btnCerrar;
        private Label lblEstado;

        public FrmReciboSocio(
            string nombreCompleto,
            string numeroSocio,
            decimal importe,
            string medioPago,
            DateTime fechaPago,
            DateTime? nuevoVencimiento)
        {
            _nombreCompleto   = nombreCompleto;
            _numeroSocio      = numeroSocio;
            _importe          = importe;
            _medioPago        = medioPago;
            _fechaPago        = fechaPago;
            _nuevoVencimiento = nuevoVencimiento;

            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Recibo de Pago de Cuota";
            this.Size = new Size(520, 540);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9.5f);

            // ── Encabezado ─────────────────────────────────────────
            panelEncabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = Color.FromArgb(110, 40, 160)
            };
            this.Controls.Add(panelEncabezado);

            lblTitulo = new Label
            {
                Text = "💰  Pago Registrado con Éxito",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 14),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblTitulo);

            lblSubtitulo = new Label
            {
                Text = $"Fecha: {_fechaPago:dd/MM/yyyy HH:mm}",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(220, 190, 255),
                Location = new Point(17, 50),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblSubtitulo);

            // ── Cuerpo (comprobante) ───────────────────────────────
            panelCuerpo = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(465, 300),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelCuerpo);

            int yPos = 20;
            AgregarSeccion(panelCuerpo, "DATOS DEL SOCIO", ref yPos);
            AgregarFila(panelCuerpo, "Nombre y Apellido:", _nombreCompleto, ref yPos);
            AgregarFila(panelCuerpo, "Nº de Socio:", _numeroSocio, ref yPos,
                Color.FromArgb(110, 40, 160), bold: true);

            yPos += 10;
            AgregarSeccion(panelCuerpo, "DATOS DEL PAGO", ref yPos);
            AgregarFila(panelCuerpo, "Importe abonado:", $"$ {_importe:N2}", ref yPos,
                Color.FromArgb(40, 40, 40), bold: true);
            AgregarFila(panelCuerpo, "Medio de pago:", _medioPago, ref yPos);
            AgregarFila(panelCuerpo, "Nuevo vencimiento:", _nuevoVencimiento.HasValue
                ? _nuevoVencimiento.Value.ToString("dd/MM/yyyy")
                : "—", ref yPos, Color.DarkRed, bold: true);

            // Nota al pie dentro del panel
            yPos += 15;
            var lblNota = new Label
            {
                Text = "Conserve este recibo como comprobante de pago.",
                Location = new Point(10, yPos),
                Size = new Size(440, 20),
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelCuerpo.Controls.Add(lblNota);

            // ── Label de estado ────────────────────────────────────
            lblEstado = new Label
            {
                Text = "",
                Location = new Point(20, 400),
                Size = new Size(465, 20),
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.DarkGreen
            };
            this.Controls.Add(lblEstado);

            // ── Botones ────────────────────────────────────────────
            btnGuardarPDF = CrearBoton("💾  Guardar PDF",
                Color.FromArgb(30, 90, 160), new Point(165, 430));
            btnCerrar = CrearBoton("✖  Cerrar",
                Color.FromArgb(190, 50, 50), new Point(330, 430));

            btnGuardarPDF.Click += BtnGuardarPDF_Click;
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.Add(btnGuardarPDF);
            this.Controls.Add(btnCerrar);
        }

        private void BtnGuardarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnGuardarPDF.Enabled = false;
                lblEstado.Text = "Generando PDF...";
                lblEstado.ForeColor = Color.FromArgb(30, 90, 160);
                this.Refresh();

                string ruta = GeneradorPDF.GenerarReciboCuota(
                    _nombreCompleto, _numeroSocio, _importe, _medioPago,
                    _fechaPago, _nuevoVencimiento);

                lblEstado.Text = $"✔ PDF guardado en: {Path.GetFileName(ruta)}";
                lblEstado.ForeColor = Color.DarkGreen;

                var resp = MessageBox.Show(
                    $"Recibo guardado correctamente.\n\nRuta: {ruta}\n\n¿Desea abrir la carpeta?",
                    "PDF generado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (resp == DialogResult.Yes)
                    GeneradorPDF.AbrirCarpeta(ruta);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al generar PDF: " + ex.Message;
                lblEstado.ForeColor = Color.DarkRed;
            }
            finally
            {
                btnGuardarPDF.Enabled = true;
            }
        }

        // ── Helpers visuales (igual que en FrmReciboAltaSocio) ────────

        private static void AgregarSeccion(Panel panel, string titulo, ref int y)
        {
            var pnl = new Panel { Location = new Point(0, y), Size = new Size(465, 22), BackColor = Color.FromArgb(230, 236, 245) };
            var lbl = new Label { Text = titulo, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold), ForeColor = Color.FromArgb(25, 80, 150), Location = new Point(8, 3), AutoSize = true };
            pnl.Controls.Add(lbl); panel.Controls.Add(pnl);
            y += 24;
        }

        private static void AgregarFila(Panel panel, string etiqueta, string valor, ref int y, Color? colorValor = null, bool bold = false)
        {
            var lblEtq = new Label { Text = etiqueta, Font = new Font("Segoe UI", 9f, FontStyle.Regular), ForeColor = Color.FromArgb(80, 80, 80), Location = new Point(12, y), Size = new Size(170, 22) };
            var lblVal = new Label { Text = valor, Font = new Font("Segoe UI", 9f, bold ? FontStyle.Bold : FontStyle.Regular), ForeColor = colorValor ?? Color.FromArgb(30, 30, 30), Location = new Point(185, y), Size = new Size(265, 22) };
            panel.Controls.Add(lblEtq); panel.Controls.Add(lblVal);
            var sep = new Label { BorderStyle = BorderStyle.Fixed3D, Location = new Point(8, y + 20), Size = new Size(445, 1) };
            panel.Controls.Add(sep);
            y += 24;
        }

        private static Button CrearBoton(string texto, Color color, Point loc)
        {
            var btn = new Button { Text = texto, Size = new Size(155, 36), Location = loc, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0; return btn;
        }

        private void InitializeComponent() { }
    }
}
