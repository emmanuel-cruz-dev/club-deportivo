using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClubDeportivo.Forms
{    
    /// Menú principal del sistema.
    /// Recibe el nombre del usuario logueado y centraliza la navegación.    
    public partial class FrmMenuPrincipal : Form
    {
        private readonly string _usuarioNombre;

        // ── Controles ────────────────────────────────────────────────
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblEstado;
        private ToolStripStatusLabel lblUsuarioActivo;
        private Panel panelBienvenida;
        private Label lblBienvenida;
        private Label lblFecha;
        private TableLayoutPanel tablaBotones;

        public FrmMenuPrincipal(string usuarioNombre)
        {
            _usuarioNombre = usuarioNombre;
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            // ── Formulario ─────────────────────────────────────────
            this.Text = "Club Deportivo – Menú Principal";
            this.Size = new Size(860, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 560);
            this.BackColor = Color.FromArgb(235, 238, 245);
            this.Font = new Font("Segoe UI", 9.5f);

            // ── Menú superior ──────────────────────────────────────
            menuStrip = new MenuStrip { BackColor = Color.FromArgb(25, 80, 150), ForeColor = Color.White };
            var mnuSocios = new ToolStripMenuItem("👤 Socios") { ForeColor = Color.White };
            mnuSocios.DropDownItems.Add("Nuevo socio", null, (s, e) => AbrirAltaSocio());
            mnuSocios.DropDownItems.Add("Listar socios", null, (s, e) => AbrirListaSocios());
            mnuSocios.DropDownItems.Add(new ToolStripSeparator());
            mnuSocios.DropDownItems.Add("Vencimientos", null, (s, e) => AbrirVencimientos());

            var mnuNoSocios = new ToolStripMenuItem("🙋 No Socios") { ForeColor = Color.White };
            mnuNoSocios.DropDownItems.Add("Nuevo no socio", null, (s, e) => MessageBox.Show("Próximamente", "Info"));

            var mnuCobros = new ToolStripMenuItem("💰 Cobros") { ForeColor = Color.White };
            mnuCobros.DropDownItems.Add("Cobrar cuota", null, (s, e) => MessageBox.Show("Próximamente", "Info"));
            mnuCobros.DropDownItems.Add("Cobrar actividad", null, (s, e) => MessageBox.Show("Próximamente", "Info"));

            var mnuSistema = new ToolStripMenuItem("⚙ Sistema") { ForeColor = Color.White };
            mnuSistema.DropDownItems.Add("Salir", null, (s, e) => Salir());

            menuStrip.Items.AddRange(new ToolStripItem[]
                { mnuSocios, mnuNoSocios, mnuCobros, mnuSistema });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // ── Panel bienvenida ────────────────────────────────────
            panelBienvenida = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(25, 80, 150),
                Padding = new Padding(20, 10, 20, 0)
            };

            var layoutBienvenida = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            layoutBienvenida.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            layoutBienvenida.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            lblBienvenida = new Label
            {
                Text = $"Bienvenido/a,  {_usuarioNombre}",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Bottom,                
            };
            lblFecha = new Label
            {
                Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy",
                            new System.Globalization.CultureInfo("es-AR")),
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(180, 210, 255),
                AutoSize = true,
                Anchor = AnchorStyles.Left | AnchorStyles.Top,              
            };            
            layoutBienvenida.Controls.Add(lblBienvenida, 0, 0);
            layoutBienvenida.Controls.Add(lblFecha, 0, 1);
            panelBienvenida.Controls.Add(layoutBienvenida);
            this.Controls.Add(panelBienvenida);

            // ── Botones grandes en grilla ──────────────────────────
            tablaBotones = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(30),
                BackColor = Color.Transparent
            };
            tablaBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            tablaBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            tablaBotones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3f));
            tablaBotones.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            tablaBotones.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            tablaBotones.Controls.Add(CrearBoton("➕  Nuevo Socio", Color.FromArgb(30, 130, 70), AbrirAltaSocio), 0, 0);
            tablaBotones.Controls.Add(CrearBoton("📋  Listar Socios", Color.FromArgb(30, 90, 160), AbrirListaSocios), 1, 0);
            tablaBotones.Controls.Add(CrearBoton("⏰  Vencimientos", Color.FromArgb(180, 100, 10), AbrirVencimientos), 2, 0);
            tablaBotones.Controls.Add(CrearBoton("💳  Cobrar Cuota", Color.FromArgb(110, 40, 160), () => MessageBox.Show("Próximamente", "Info")), 0, 1);
            tablaBotones.Controls.Add(CrearBoton("🙋  No Socios", Color.FromArgb(60, 120, 130), () => MessageBox.Show("Próximamente", "Info")), 1, 1);
            tablaBotones.Controls.Add(CrearBoton("🚪  Salir", Color.FromArgb(190, 50, 50), Salir), 2, 1);

            this.Controls.Add(tablaBotones);
            tablaBotones.BringToFront();

            // ── Barra de estado ────────────────────────────────────
            statusStrip = new StatusStrip { BackColor = Color.FromArgb(25, 80, 150) };
            lblEstado = new ToolStripStatusLabel("Listo")
            { ForeColor = Color.White, Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            lblUsuarioActivo = new ToolStripStatusLabel($"Usuario: {_usuarioNombre}")
            { ForeColor = Color.LightYellow };
            statusStrip.Items.AddRange(new ToolStripItem[] { lblEstado, lblUsuarioActivo });
            this.Controls.Add(statusStrip);
        }

        // ── Helper: crear botón de menú ────────────────────────────
        private Button CrearBoton(string texto, Color color, Action accion)
        {
            var btn = new Button
            {
                Text = texto,
                Dock = DockStyle.Fill,
                Margin = new Padding(8),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => accion?.Invoke();
            // Efecto hover
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(color, 0.15f);
            btn.MouseLeave += (s, e) => btn.BackColor = color;
            return btn;
        }

        // ── Navegación ─────────────────────────────────────────────
        private void AbrirAltaSocio()
        {
            using var frm = new FrmAltaSocio();
            frm.ShowDialog(this);            
        }

        private void AbrirListaSocios()
        {
            using var frm = new FrmListaSocios();
            frm.ShowDialog(this);            
        }

        private void AbrirVencimientos()
        {
            using var frm = new FrmVencimientos();
            frm.ShowDialog(this);
        }

        private void Salir()
        {
            if (MessageBox.Show("¿Deseás cerrar la sesión?", "Salir",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void InitializeComponent() { }
    }
}