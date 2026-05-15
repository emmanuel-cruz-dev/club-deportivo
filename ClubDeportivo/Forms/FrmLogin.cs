using System;
using System.Drawing;
using System.Windows.Forms;
using ClubDeportivo.DAL;

namespace ClubDeportivo.Forms
{    
    /// Pantalla de Login del sistema.    
    public partial class FrmLogin : Form
    {
        private readonly UsuarioDAL _usuarioDAL = new UsuarioDAL();
                
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblUsuario;
        private Label lblContrasena;
        private TextBox txtUsuario;
        private TextBox txtContrasena;
        private Button btnIngresar;
        private Button btnSalir;
        private PictureBox picLogo;
        private Panel panelCentral;
        private Label lblError;

        public FrmLogin()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Club Deportivo – Acceso al sistema";
            this.Size = new Size(500, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new Size(480, 480);
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9f);

            panelCentral = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 8,
                BackColor = Color.Transparent
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 10));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            lblTitulo = new Label
            {
                Text = "🏟  CLUB DEPORTIVO",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 90, 160),
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            mainLayout.Controls.Add(lblTitulo, 0, 0);

            lblSubtitulo = new Label
            {
                Text = "Sistema de Gestión",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top
            };
            mainLayout.Controls.Add(lblSubtitulo, 0, 1);

            var sep = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Dock = DockStyle.Top,
                Height = 2
            };
            mainLayout.Controls.Add(sep, 0, 2);

            lblUsuario = new Label
            {
                Text = "Usuario:",
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            mainLayout.Controls.Add(lblUsuario, 0, 3);

            txtUsuario = new TextBox
            {
                Font = new Font("Segoe UI", 10f),
                Dock = DockStyle.Top,
                Height = 30
            };
            mainLayout.Controls.Add(txtUsuario, 0, 4);

            lblContrasena = new Label
            {
                Text = "Contraseña:",
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom
            };
            mainLayout.Controls.Add(lblContrasena, 0, 5);

            txtContrasena = new TextBox
            {
                Font = new Font("Segoe UI", 10f),
                PasswordChar = '●',
                Dock = DockStyle.Top,
                Height = 30
            };
            mainLayout.Controls.Add(txtContrasena, 0, 6);

            var panelInferior = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.Transparent
            };

            panelInferior.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            panelInferior.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            panelInferior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            panelInferior.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            lblError = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8.5f),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 10, 0, 0)
            };
            panelInferior.Controls.Add(lblError, 0, 0);
            panelInferior.SetColumnSpan(lblError, 2);

            btnIngresar = new Button
            {
                Text = "Ingresar",
                BackColor = Color.FromArgb(30, 90, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 5, 10, 5)
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Click += BtnIngresar_Click;
            panelInferior.Controls.Add(btnIngresar, 0, 1);

            btnSalir = new Button
            {
                Text = "Salir",
                BackColor = Color.FromArgb(210, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f),
                Cursor = Cursors.Hand,
                Dock = DockStyle.Fill,
                Margin = new Padding(10, 5, 0, 5)
            };
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.Click += (s, e) => Application.Exit();
            panelInferior.Controls.Add(btnSalir, 1, 1);

            mainLayout.Controls.Add(panelInferior, 0, 7);

            panelCentral.Controls.Add(mainLayout);
            this.Controls.Add(panelCentral);

            // Eventos de teclado
            txtContrasena.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) BtnIngresar_Click(s, e);
            };
            txtUsuario.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) txtContrasena.Focus();
            };

            this.ActiveControl = txtUsuario;
        }

        // ── Lógica de Login ───────────────────────────────────────────
        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
            {
                lblError.Text = "⚠ Completá usuario y contraseña.";
                return;
            }

            try
            {
                btnIngresar.Enabled = false;
                btnIngresar.Text = "Validando...";

                string nombreCompleto = _usuarioDAL.ValidarLogin(usuario, contrasena);

                if (nombreCompleto != null)
                {
                    var menu = new FrmMenuPrincipal(nombreCompleto);
                    menu.Show();
                    this.Hide();
                    menu.FormClosed += (s2, e2) => this.Close();
                }
                else
                {
                    lblError.Text = "⚠ Usuario o contraseña incorrectos.";
                    txtContrasena.Clear();
                    txtContrasena.Focus();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"⚠ Error de conexión: {ex.Message}";
            }
            finally
            {
                btnIngresar.Enabled = true;
                btnIngresar.Text = "Ingresar";
            }
        }
        
        private void InitializeComponent() { }
    }
}