using System;
using System.Drawing;
using System.Windows.Forms;
using ClubDeportivo.Tests;

namespace ClubDeportivo.Forms
{
    /// <summary>
    /// Ventana que simula una consola para mostrar los resultados
    /// de las pruebas del sistema.
    /// </summary>
    public partial class FrmPruebas : Form
    {
        private RichTextBox rtbConsola;
        private Button btnCerrar, btnReejecutar;
        private Label lblTitulo;

        public FrmPruebas()
        {
            InitializeComponent();
            ConfigurarFormulario();
            Ejecutar();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Pruebas del Sistema – Modo Consola";
            this.Size = new Size(730, 620);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.Font = new Font("Consolas", 10f);

            lblTitulo = new Label
            {
                Text = "🧪 EJECUCIÓN DE PRUEBAS UNITARIAS Y LÓGICAS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            rtbConsola = new RichTextBox
            {
                Location = new Point(15, 60),
                Size = new Size(665, 430),
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(10)
            };
            this.Controls.Add(rtbConsola);

            btnReejecutar = new Button
            {
                Text = "↺ Volver a Ejecutar",
                Size = new Size(260, 36),
                Location = new Point(265, 505),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReejecutar.Click += (s, e) => Ejecutar();
            this.Controls.Add(btnReejecutar);

            btnCerrar = new Button
            {
                Text = "✖ Cerrar",
                Size = new Size(130, 36),
                Location = new Point(550, 505),
                BackColor = Color.FromArgb(190, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);
        }

        private void Ejecutar()
        {
            rtbConsola.Clear();
            rtbConsola.AppendText("Iniciando pruebas...\n");

            var motor = new PruebasSistema();
            string resultados = motor.EjecutarPruebas();

            rtbConsola.AppendText(resultados);
            rtbConsola.SelectionStart = rtbConsola.Text.Length;
            rtbConsola.ScrollToCaret();
        }

        private void InitializeComponent() { }
    }
}
