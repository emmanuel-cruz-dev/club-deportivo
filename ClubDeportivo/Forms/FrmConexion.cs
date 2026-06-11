using System;
using System.Drawing;
using System.Windows.Forms;
using ClubDeportivo.Data;

namespace ClubDeportivo.Forms
{
    /// <summary>
    /// Formulario de configuración de conexión a la base de datos.
    /// Solicita T_SERVER, T_USER, T_PASSWORD y T_PORT al iniciar la aplicación.
    /// </summary>
    public class FrmConexion : Form
    {
        // ── Controles ────────────────────────────────────────────────────────────
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblServer;
        private Label lblUser;
        private Label lblPassword;
        private Label lblPort;
        private TextBox txtServer;
        private TextBox txtUser;
        private TextBox txtPassword;
        private TextBox txtPort;
        private Button btnConectar;
        private Button btnCancelar;
        private Panel pnlHeader;

        public FrmConexion()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Form ─────────────────────────────────────────────────────────────
            this.Text = "Configuración de Conexión — Club Deportivo";
            this.Size = new Size(480, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 9.5f);

            // ── Header panel ─────────────────────────────────────────────────────
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(30, 80, 160)
            };

            lblTitulo = new Label
            {
                Text = "Club Deportivo",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Location = new Point(16, 10),
                AutoSize = true
            };

            lblSubtitulo = new Label
            {
                Text = "Configuración de conexión a la base de datos",
                ForeColor = Color.FromArgb(180, 210, 255),
                Font = new Font("Segoe UI", 8.5f),
                Location = new Point(18, 48),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(lblSubtitulo);

            // ── Helper: crear label ───────────────────────────────────────────────
            Label MkLabel(string text, int top) => new Label
            {
                Text = text,
                Location = new Point(30, top),
                Size = new Size(120, 22),
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(50, 60, 80),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            // ── Helper: crear textbox ─────────────────────────────────────────────
            TextBox MkText(int top, bool password = false) => new TextBox
            {
                Location = new Point(160, top),
                Size = new Size(210, 28),
                PasswordChar = password ? '●' : '\0',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            int baseY = 90;
            int step  = 50;

            lblServer   = MkLabel("SERVER :",   baseY);
            lblUser     = MkLabel("USER :",     baseY + step);
            lblPassword = MkLabel("PASSWORD :", baseY + step * 2);
            lblPort     = MkLabel("PORT :",     baseY + step * 3);

            txtServer   = MkText(baseY);
            txtUser     = MkText(baseY + step);
            txtPassword = MkText(baseY + step * 2, password: true);
            txtPort     = MkText(baseY + step * 3);

            // Valores por defecto
            txtServer.Text = "localhost";
            txtPort.Text   = "3306";

            // ── Botones ───────────────────────────────────────────────────────────
            btnConectar = new Button
            {
                Text = "Conectar",
                Location = new Point(160, baseY + step * 4 + 10),
                Size = new Size(100, 34),
                BackColor = Color.FromArgb(30, 80, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConectar.FlatAppearance.BorderSize = 0;
            btnConectar.Click += BtnConectar_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(270, baseY + step * 4 + 10),
                Size = new Size(100, 34),
                BackColor = Color.FromArgb(200, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            // ── Agregar controles ─────────────────────────────────────────────────
            this.Controls.Add(pnlHeader);
            this.Controls.Add(lblServer);
            this.Controls.Add(lblUser);
            this.Controls.Add(lblPassword);
            this.Controls.Add(lblPort);
            this.Controls.Add(txtServer);
            this.Controls.Add(txtUser);
            this.Controls.Add(txtPassword);
            this.Controls.Add(txtPort);
            this.Controls.Add(btnConectar);
            this.Controls.Add(btnCancelar);

            // Enter dispara conectar
            this.AcceptButton = btnConectar;
        }

        private void BtnConectar_Click(object sender, EventArgs e)
        {
            // ── Validaciones básicas ──────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(txtServer.Text))
            {
                MostrarError("El campo T_SERVER es obligatorio.");
                txtServer.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtUser.Text))
            {
                MostrarError("El campo T_USER es obligatorio.");
                txtUser.Focus();
                return;
            }
            if (!int.TryParse(txtPort.Text.Trim(), out int puerto) || puerto <= 0 || puerto > 65535)
            {
                MostrarError("El campo T_PORT debe ser un número de puerto válido (ej: 3306).");
                txtPort.Focus();
                return;
            }

            // ── Configurar y probar conexión ──────────────────────────────────────
            Conexion.Configurar(
                tServer:   txtServer.Text.Trim(),
                tUser:     txtUser.Text.Trim(),
                tPassword: txtPassword.Text,
                tPort:     puerto
            );

            if (Conexion.Instancia.ProbarConexion(out string msg))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MostrarError($"No se pudo conectar a la base de datos:\n\n{msg}\n\nVerificá los datos e intentá nuevamente.");
            }
        }

        private static void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error de conexión",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
