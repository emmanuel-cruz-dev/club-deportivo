using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClubDeportivo.Helpers;

namespace ClubDeportivo.Forms
{
    /// <summary>
    /// Formulario de salida: muestra el comprobante de alta de socio
    /// y permite guardar el PDF o simplemente cerrarlo.
    /// </summary>
    public partial class FrmReciboAltaSocio : Form
    {
        // ── Datos del recibo ──────────────────────────────────────────
        private readonly string _nombreCompleto;
        private readonly string _dni;
        private readonly string _numeroSocio;
        private readonly string _numeroCarnet;
        private readonly DateTime _fechaAlta;
        private readonly decimal _importeCuota;
        private readonly string _medioPago;

        // ── Controles ────────────────────────────────────────────────
        private Panel panelEncabezado;
        private Label lblTitulo, lblSubtitulo;
        private Panel panelCuerpo;
        private Button btnGuardarPDF, btnCerrar;
        private Label lblEstado;

        public FrmReciboAltaSocio(
            string nombreCompleto,
            string dni,
            string numeroSocio,
            string numeroCarnet,
            DateTime fechaAlta,
            decimal importeCuota,
            string medioPago)
        {
            _nombreCompleto = nombreCompleto;
            _dni            = dni;
            _numeroSocio    = numeroSocio;
            _numeroCarnet   = numeroCarnet;
            _fechaAlta      = fechaAlta;
            _importeCuota   = importeCuota;
            _medioPago      = medioPago;

            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Comprobante de Alta de Socio";
            this.Size = new Size(520, 560);
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
                BackColor = Color.FromArgb(30, 130, 70)
            };
            this.Controls.Add(panelEncabezado);

            lblTitulo = new Label
            {
                Text = "✅  Socio Registrado Exitosamente",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 14),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblTitulo);

            lblSubtitulo = new Label
            {
                Text = $"Fecha: {_fechaAlta:dd/MM/yyyy HH:mm}",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(200, 240, 210),
                Location = new Point(17, 50),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblSubtitulo);

            // ── Cuerpo (comprobante) ───────────────────────────────
            panelCuerpo = new Panel
            {
                Location = new Point(20, 90),
                Size = new Size(465, 340),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelCuerpo);

            int yPos = 20;
            AgregarSeccion(panelCuerpo, "DATOS DEL SOCIO", ref yPos);
            AgregarFila(panelCuerpo, "Nombre y Apellido:", _nombreCompleto, ref yPos);
            AgregarFila(panelCuerpo, "DNI:", _dni, ref yPos);
            AgregarFila(panelCuerpo, "Nº de Socio:", _numeroSocio, ref yPos,
                Color.FromArgb(30, 130, 70), bold: true);
            AgregarFila(panelCuerpo, "Nº de Carnet:", _numeroCarnet, ref yPos,
                Color.FromArgb(30, 90, 160), bold: true);
            AgregarFila(panelCuerpo, "Fecha de Alta:", _fechaAlta.ToString("dd/MM/yyyy"), ref yPos);

            yPos += 10;
            AgregarSeccion(panelCuerpo, "DATOS DEL PAGO", ref yPos);
            AgregarFila(panelCuerpo, "Cuota inicial:", $"$ {_importeCuota:N2}", ref yPos,
                Color.FromArgb(40, 40, 40), bold: true);
            AgregarFila(panelCuerpo, "Medio de pago:", _medioPago, ref yPos);

            // Nota al pie dentro del panel
            yPos += 15;
            var lblNota = new Label
            {
                Text = "Este documento acredita el alta como socio del Club Deportivo.",
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
                Location = new Point(20, 440),
                Size = new Size(465, 20),
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.DarkGreen
            };
            this.Controls.Add(lblEstado);

            // ── Botones ────────────────────────────────────────────
            btnGuardarPDF = CrearBoton("💾  Guardar PDF",
                Color.FromArgb(30, 90, 160), new Point(175, 462));
            btnCerrar = CrearBoton("✖  Cerrar",
                Color.FromArgb(190, 50, 50), new Point(340, 462));

            btnGuardarPDF.Click += BtnGuardarPDF_Click;
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.Add(btnGuardarPDF);
            this.Controls.Add(btnCerrar);
        }

        // ── Guardar PDF ───────────────────────────────────────────────
        private void BtnGuardarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                btnGuardarPDF.Enabled = false;
                lblEstado.Text = "Generando PDF...";
                lblEstado.ForeColor = Color.FromArgb(30, 90, 160);
                this.Refresh();

                string ruta = GeneradorPDF.GenerarReciboAltaSocio(
                    _nombreCompleto, _dni, _numeroSocio, _numeroCarnet,
                    _fechaAlta, _importeCuota, _medioPago);

                lblEstado.Text = $"✔ PDF guardado en: {Path.GetFileName(ruta)}";
                lblEstado.ForeColor = Color.DarkGreen;

                var resp = MessageBox.Show(
                    $"PDF guardado correctamente.\n\nRuta: {ruta}\n\n¿Desea abrir la carpeta?",
                    "PDF generado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (resp == DialogResult.Yes)
                    GeneradorPDF.AbrirCarpeta(ruta);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al generar PDF: " + ex.Message;
                MessageBox.Show("Error al generar PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblEstado.ForeColor = Color.DarkRed;
            }
            finally
            {
                btnGuardarPDF.Enabled = true;
            }
        }

        // ── Helpers visuales ─────────────────────────────────────────

        private static void AgregarSeccion(Panel panel, string titulo, ref int y)
        {
            var pnl = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(465, 22),
                BackColor = Color.FromArgb(230, 236, 245)
            };
            var lbl = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 80, 150),
                Location = new Point(8, 3),
                AutoSize = true
            };
            pnl.Controls.Add(lbl);
            panel.Controls.Add(pnl);
            y += 24;
        }

        private static void AgregarFila(Panel panel, string etiqueta, string valor,
            ref int y, Color? colorValor = null, bool bold = false)
        {
            var lblEtq = new Label
            {
                Text = etiqueta,
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(12, y),
                Size = new Size(170, 22)
            };
            var lblVal = new Label
            {
                Text = valor,
                Font = new Font("Segoe UI", 9f, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = colorValor ?? Color.FromArgb(30, 30, 30),
                Location = new Point(185, y),
                Size = new Size(265, 22)
            };
            panel.Controls.Add(lblEtq);
            panel.Controls.Add(lblVal);

            // Línea divisora
            var sep = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(8, y + 20),
                Size = new Size(445, 1)
            };
            panel.Controls.Add(sep);
            y += 24;
        }

        private static Button CrearBoton(string texto, Color color, Point loc)
        {
            var btn = new Button
            {
                Text = texto,
                Size = new Size(155, 36),
                Location = loc,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void InitializeComponent() { }
    }
}
