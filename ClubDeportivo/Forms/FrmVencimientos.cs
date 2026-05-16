using System;
using ClubDeportivo.DAL;
using ClubDeportivo.Models;

namespace ClubDeportivo.Forms
{    
    /// Muestra socios con cuota vencida o próxima a vencer.
    /// Permite configurar cuántos días de anticipación mostrar.    
    public partial class FrmVencimientos : Form
    {
        private readonly SocioDAL _socioDAL = new SocioDAL();

        private Panel panelTop;
        private Label lblTitulo;
        private Label lblDias;
        private NumericUpDown nudDias;
        private Button btnConsultar, btnCerrar;
        private DataGridView dgvVencimientos;
        private Label lblResumen;

        public FrmVencimientos()
        {
            InitializeComponent();
            ConfigurarFormulario();
            Consultar(7); // carga inicial: próximos 7 días
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Consulta de Vencimientos";
            this.Size = new Size(860, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9.5f);

            // ── Encabezado ─────────────────────────────────────────
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(180, 100, 10)
            };
            this.Controls.Add(panelTop);

            lblTitulo = new Label
            {
                Text = "⏰  Vencimientos de Cuotas",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 17),
                AutoSize = true
            };
            panelTop.Controls.Add(lblTitulo);

            // ── Barra de filtro ────────────────────────────────────
            var panelFiltro = new Panel
            {
                Location = new Point(0, 65),
                Size = new Size(this.ClientSize.Width, 50),
                BackColor = Color.White
            };
            this.Controls.Add(panelFiltro);

            lblDias = new Label
            {
                Text = "Mostrar vencimientos en los próximos",
                Location = new Point(15, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f)
            };
            panelFiltro.Controls.Add(lblDias);

            nudDias = new NumericUpDown
            {
                Location = new Point(280, 11),
                Size = new Size(60, 24),
                Minimum = 0,
                Maximum = 90,
                Value = 7,
                Font = new Font("Segoe UI", 10f)
            };
            panelFiltro.Controls.Add(nudDias);

            var lblDiasSuf = new Label
            {
                Text = "días  (0 = solo vencidos)",
                Location = new Point(348, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Gray
            };
            panelFiltro.Controls.Add(lblDiasSuf);

            btnConsultar = new Button
            {
                Text = "🔍 Consultar",
                Size = new Size(120, 30),
                Location = new Point(580, 10),
                BackColor = Color.FromArgb(180, 100, 10),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConsultar.FlatAppearance.BorderSize = 0;
            btnConsultar.Click += (s, e) => Consultar((int)nudDias.Value);
            panelFiltro.Controls.Add(btnConsultar);

            // ── Leyenda de colores ─────────────────────────────────
            var panelLeyenda = new Panel
            {
                Location = new Point(0, 115),
                Size = new Size(this.ClientSize.Width, 28),
                BackColor = Color.FromArgb(250, 250, 250)
            };
            this.Controls.Add(panelLeyenda);

            AgregarLeyenda(panelLeyenda, "■ Vencido / Suspendido", Color.FromArgb(220, 50, 50), 10);
            AgregarLeyenda(panelLeyenda, "■ Vence en ≤ 3 días", Color.FromArgb(220, 120, 0), 200);
            AgregarLeyenda(panelLeyenda, "■ Vence en 4–7 días", Color.FromArgb(30, 130, 70), 380);

            // ── DataGridView ───────────────────────────────────────
            dgvVencimientos = new DataGridView
            {
                Location = new Point(10, 148),
                Size = new Size(this.ClientSize.Width - 20, 300),
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
                ColumnHeadersHeight = 36
            };
            dgvVencimientos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(180, 100, 10);
            dgvVencimientos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVencimientos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvVencimientos.EnableHeadersVisualStyles = false;
            this.Controls.Add(dgvVencimientos);

            ConfigurarColumnas();

            // ── Pie ────────────────────────────────────────────────
            lblResumen = new Label
            {
                Text = "",
                Location = new Point(15, 460),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            this.Controls.Add(lblResumen);

            btnCerrar = new Button
            {
                Text = "✖ Cerrar",
                Size = new Size(100, 32),
                Location = new Point(735, 456),
                BackColor = Color.FromArgb(190, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCerrar);
        }

        private void ConfigurarColumnas()
        {
            dgvVencimientos.Columns.Clear();
            dgvVencimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "NumeroSocio", HeaderText = "Nº Socio", FillWeight = 10 });
            dgvVencimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellido", HeaderText = "Apellido", FillWeight = 16 });
            dgvVencimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre", FillWeight = 16 });
            dgvVencimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "DNI", HeaderText = "DNI", FillWeight = 12 });
            dgvVencimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Vencimiento", HeaderText = "Venc. Cuota", FillWeight = 12 });
            dgvVencimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiasParaVencer", HeaderText = "Días", FillWeight = 8 });
            dgvVencimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", FillWeight = 10 });
            dgvVencimientos.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Teléfono", FillWeight = 12 });

            foreach (DataGridViewColumn col in dgvVencimientos.Columns)
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void Consultar(int dias)
        {
            try
            {
                var lista = _socioDAL.ConsultarVencimientos(dias);
                dgvVencimientos.Rows.Clear();

                int vencidos = 0;
                int proximos = 0;

                foreach (var s in lista)
                {
                    int diasRestantes = s.FechaVencimientoCuota.HasValue
                        ? (int)(s.FechaVencimientoCuota.Value - DateTime.Today).TotalDays
                        : -999;

                    string diasStr = diasRestantes < 0
                        ? $"Vencido ({Math.Abs(diasRestantes)}d)"
                        : $"{diasRestantes}d";

                    int idx = dgvVencimientos.Rows.Add(
                        s.NumeroSocio,
                        s.Apellido,
                        s.Nombre,
                        s.DNI,
                        s.FechaVencimientoCuota.HasValue
                            ? s.FechaVencimientoCuota.Value.ToString("dd/MM/yyyy")
                            : "-",
                        diasStr,
                        s.Estado.ToString(),
                        s.Telefono
                    );

                    var row = dgvVencimientos.Rows[idx];
                    if (s.Estado == EstadoSocio.Suspendido || diasRestantes < 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                        vencidos++;
                    }
                    else if (diasRestantes <= 3)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 180);
                        row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                        proximos++;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(210, 245, 220);
                        row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                        proximos++;
                    }
                }

                lblResumen.Text = $"Total: {lista.Count} registro(s)  |  " +
                                  $"Vencidos/Suspendidos: {vencidos}  |  " +
                                  $"Próximos a vencer: {proximos}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar vencimientos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void AgregarLeyenda(Panel panel, string texto, Color color, int x)
        {
            var lbl = new Label
            {
                Text = texto,
                ForeColor = color,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Location = new Point(x, 6),
                AutoSize = true
            };
            panel.Controls.Add(lbl);
        }

        private void InitializeComponent() { }
    }
}
