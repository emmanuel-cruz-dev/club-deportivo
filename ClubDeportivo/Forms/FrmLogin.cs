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
            this.Size = new Size(480, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9f);
                        
            panelCentral = new Panel
            {
                Size = new Size(360, 310),
                Location = new Point(60, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelCentral);
                        
            lblTitulo = new Label
            {
                Text = "🏟  CLUB DEPORTIVO",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 90, 160),
                Location = new Point(40, 20),
                AutoSize = true
            };
            panelCentral.Controls.Add(lblTitulo);

            lblSubtitulo = new Label
            {
                Text = "Sistema de Gestión",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(40, 50),
                AutoSize = true
            };
            panelCentral.Controls.Add(lblSubtitulo);
                        
            var sep = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Size = new Size(280, 2),
                Location = new Point(40, 75)
            };
            panelCentral.Controls.Add(sep);
            
            lblUsuario = new Label
            {
                Text = "Usuario:",
                Location = new Point(40, 95),
                AutoSize = true
            };
            panelCentral.Controls.Add(lblUsuario);

            txtUsuario = new TextBox
            {
                Size = new Size(280, 30),
                Location = new Point(40, 115),
                Font = new Font("Segoe UI", 10f)
            };
            panelCentral.Controls.Add(txtUsuario);
            
            lblContrasena = new Label
            {
                Text = "Contraseña:",
                Location = new Point(40, 155),
                AutoSize = true
            };
            panelCentral.Controls.Add(lblContrasena);

            txtContrasena = new TextBox
            {
                Size = new Size(280, 30),
                Location = new Point(40, 175),
                PasswordChar = '●',
                Font = new Font("Segoe UI", 10f)
            };
            panelCentral.Controls.Add(txtContrasena);
            
            lblError = new Label
            {
                Text = "",
                ForeColor = Color.Red,
                Location = new Point(40, 210),
                Size = new Size(280, 20),
                Font = new Font("Segoe UI", 8.5f)
            };
            panelCentral.Controls.Add(lblError);

            btnIngresar = new Button
            {
                Text = "Ingresar",
                Size = new Size(130, 40),
                Location = new Point(40, 240),
                BackColor = Color.FromArgb(30, 90, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Click += BtnIngresar_Click;
            panelCentral.Controls.Add(btnIngresar);
            
            btnSalir = new Button
            {
                Text = "Salir",
                Size = new Size(110, 40),
                Location = new Point(190, 240),
                BackColor = Color.FromArgb(210, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f),
                Cursor = Cursors.Hand
            };
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.Click += (s, e) => Application.Exit();
            panelCentral.Controls.Add(btnSalir);
           
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