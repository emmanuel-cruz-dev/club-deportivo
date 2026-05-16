using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ClubDeportivo.DAL;
using ClubDeportivo.Models;

namespace ClubDeportivo.Forms
{    
    /// Muestra el listado completo de socios con búsqueda por DNI, número o apellido.
    public partial class FrmListaSocios : Form
    {
        private readonly SocioDAL _socioDAL = new SocioDAL();

        private Panel panelTop;
        private Label lblTitulo;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Button btnBuscar, btnTodos, btnCerrar;
        private DataGridView dgvSocios;
        private Label lblTotal;

        public FrmListaSocios()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarTodos();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Listado de Socios";
            this.Size = new Size(950, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9.5f);

            // ── Panel superior ─────────────────────────────────────
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(25, 80, 150),
                Padding = new Padding(15, 0, 15, 0)
            };
            this.Controls.Add(panelTop);

            lblTitulo = new Label
            {
                Text = "📋  Listado de Socios",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 18),
                AutoSize = true
            };
            panelTop.Controls.Add(lblTitulo);

            // ── Barra de búsqueda ──────────────────────────────────
            var panelBusq = new Panel
            {
                Location = new Point(0, 70),
                Size = new Size(this.ClientSize.Width, 60),
                BackColor = Color.White,
                Padding = new Padding(10, 8, 10, 8)
            };
            this.Controls.Add(panelBusq);

            lblBuscar = new Label
            {
                Text = "Buscar (DNI / Nº Socio / Apellido):",
                Location = new Point(15, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f)
            };
            panelBusq.Controls.Add(lblBuscar);

            txtBuscar = new TextBox
            {
                Location = new Point(230, 16),
                Size = new Size(320, 28),
                Font = new Font("Segoe UI", 10f)
            };
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnBuscar_Click(s, e); };
            panelBusq.Controls.Add(txtBuscar);

            btnBuscar = CrearBoton("🔍 Buscar", Color.FromArgb(30, 90, 160), new Point(565, 14), 100);
            btnBuscar.Click += BtnBuscar_Click;
            panelBusq.Controls.Add(btnBuscar);

            btnTodos = CrearBoton("↺ Todos", Color.FromArgb(90, 100, 110), new Point(685, 14), 90);
            btnTodos.Click += (s, e) => { txtBuscar.Clear(); CargarTodos(); };
            panelBusq.Controls.Add(btnTodos);

            // ── DataGridView ───────────────────────────────────────
            dgvSocios = new DataGridView
            {
                Location = new Point(10, 140),
                Size = new Size(this.ClientSize.Width - 20, 330),

                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,

                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,

                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,

                Font = new Font("Segoe UI", 9f),

                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,

                ColumnHeadersHeight = 52,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };

            dgvSocios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(25, 80, 150);
            dgvSocios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSocios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvSocios.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSocios.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvSocios.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 4, 0, 4);            
            dgvSocios.EnableHeadersVisualStyles = false;
            dgvSocios.RowTemplate.Height = 32;
            dgvSocios.ColumnHeadersHeight = 40;
            dgvSocios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            this.Controls.Add(dgvSocios);

            ConfigurarColumnas();

            // ── Pie ────────────────────────────────────────────────
            lblTotal = new Label
            {
                Text = "",
                Location = new Point(15, 490),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            this.Controls.Add(lblTotal);

            btnCerrar = CrearBoton("✖ Cerrar", Color.FromArgb(190, 50, 50), new Point(770, 500), 120);
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);
        }

        private void ConfigurarColumnas()
        {
            dgvSocios.Columns.Clear();
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "NumeroSocio", HeaderText = "Nº Socio", FillWeight = 10 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellido", HeaderText = "Apellido", FillWeight = 15 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", FillWeight = 15 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "DNI", HeaderText = "DNI", FillWeight = 12 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", FillWeight = 12 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaAlta", HeaderText = "Alta", FillWeight = 10 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vencimiento", HeaderText = "Venc. Cuota", FillWeight = 14 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", FillWeight = 10 });
            dgvSocios.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apto", HeaderText = "Apto Físico", FillWeight = 10 });

            foreach (DataGridViewColumn col in dgvSocios.Columns)
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void CargarTodos()
        {
            try
            {
                var lista = _socioDAL.ObtenerSocios();
                MostrarEnGrilla(lista);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar socios: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(busqueda)) { CargarTodos(); return; }

            try
            {
                var lista = _socioDAL.BuscarSocio(busqueda);
                MostrarEnGrilla(lista);
                if (lista.Count == 0)
                    MessageBox.Show("No se encontraron socios con ese criterio.", "Sin resultados",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarEnGrilla(List<Socio> lista)
        {
            dgvSocios.Rows.Clear();
            foreach (var s in lista)
            {
                int idx = dgvSocios.Rows.Add(
                    s.NumeroSocio,
                    s.Apellido,
                    s.Nombre,
                    s.DNI,
                    s.Telefono,
                    s.FechaAlta.ToString("dd/MM/yyyy"),
                    s.FechaVencimientoCuota.HasValue
                        ? s.FechaVencimientoCuota.Value.ToString("dd/MM/yyyy")
                        : "-",
                    s.Estado.ToString(),
                    s.AptoFisicoPresentado ? "✔" : "✘"
                );

                // Colorear según estado
                var row = dgvSocios.Rows[idx];
                if (s.Estado == EstadoSocio.Suspendido)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else if (s.FechaVencimientoCuota.HasValue &&
                         s.FechaVencimientoCuota.Value <= DateTime.Today.AddDays(7))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 200);
                    row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                }
            }

            lblTotal.Text = $"Total: {lista.Count} socio(s)";
        }

        private Button CrearBoton(string texto, Color color, Point location, int ancho)
        {
            var btn = new Button
            {
                Text = texto,
                Size = new Size(ancho, 30),
                Location = location,
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