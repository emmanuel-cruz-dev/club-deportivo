using System;
using ClubDeportivo.DAL;
using ClubDeportivo.Models;

namespace ClubDeportivo.Forms
{
    /// Permite buscar un socio y registrar el pago de su cuota mensual.
    /// Llama al stored procedure sp_CobrarCuota.
    public partial class FrmCobrarCuota : Form
    {
        private readonly SocioDAL _socioDAL = new SocioDAL();

        // ── Controles ────────────────────────────────────────────────
        private Label lblTitulo;
        private GroupBox grpBuscar, grpSocio, grpPago;

        // Búsqueda
        private TextBox txtBuscar;
        private Button btnBuscar;

        // Datos del socio (sólo lectura)
        private Label lblNomSocio, lblNumSocio, lblEstadoSocio, lblVencSocio;
        private TextBox txtNomSocio, txtNumSocio, txtEstadoSocio, txtVencSocio;

        // Datos del pago
        private Label lblImporte, lblMedioPago, lblCantCuotas;
        private TextBox txtImporte;
        private ComboBox cmbMedioPago, cmbCantCuotas;

        private Button btnRegistrar, btnCancelar;
        private Label lblEstado;

        // Socio actualmente cargado
        private Socio _socioActual = null;

        public FrmCobrarCuota()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Cobro de Cuota Mensual";
            this.Size = new Size(620, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9.5f);

            lblTitulo = new Label
            {
                Text = "💳  Cobro de Cuota Mensual",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 40, 160),
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            // ── GroupBox Buscar Socio ──────────────────────────────
            grpBuscar = new GroupBox
            {
                Text = "Buscar Socio",
                Location = new Point(15, 50),
                Size = new Size(575, 60),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 40, 160)
            };
            this.Controls.Add(grpBuscar);

            var lblB = new Label { Text = "DNI / Nº Socio / Apellido:", Location = new Point(10, 25), AutoSize = true };
            grpBuscar.Controls.Add(lblB);

            txtBuscar = new TextBox
            {
                Location = new Point(190, 21),
                Size = new Size(260, 24),
                Font = new Font("Segoe UI", 10f)
            };
            txtBuscar.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnBuscar_Click(s, e); };
            grpBuscar.Controls.Add(txtBuscar);

            btnBuscar = new Button
            {
                Text = "🔍 Buscar",
                Size = new Size(100, 26),
                Location = new Point(460, 20),
                BackColor = Color.FromArgb(110, 40, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Click += BtnBuscar_Click;
            grpBuscar.Controls.Add(btnBuscar);

            // ── GroupBox Datos Socio ───────────────────────────────
            grpSocio = new GroupBox
            {
                Text = "Datos del Socio",
                Location = new Point(15, 122),
                Size = new Size(575, 100),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 40, 160)
            };
            this.Controls.Add(grpSocio);

            AgregarParLabel(grpSocio, "Nombre:", 10, 25, out lblNomSocio, out txtNomSocio, 260);
            AgregarParLabel(grpSocio, "Nº Socio:", 290, 25, out lblNumSocio, out txtNumSocio, 155);
            AgregarParLabel(grpSocio, "Estado:", 10, 65, out lblEstadoSocio, out txtEstadoSocio, 120);
            AgregarParLabel(grpSocio, "Venc. cuota:", 200, 65, out lblVencSocio, out txtVencSocio, 155);

            txtNomSocio.ReadOnly = txtNumSocio.ReadOnly =
            txtEstadoSocio.ReadOnly = txtVencSocio.ReadOnly = true;
            txtNomSocio.BackColor = txtNumSocio.BackColor =
            txtEstadoSocio.BackColor = txtVencSocio.BackColor = Color.FromArgb(245, 245, 250);

            // ── GroupBox Datos del Pago ────────────────────────────
            grpPago = new GroupBox
            {
                Text = "Datos del Pago",
                Location = new Point(15, 232),
                Size = new Size(575, 110),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 40, 160)
            };
            this.Controls.Add(grpPago);

            // Importe
            lblImporte = new Label { Text = "Importe ($) *:", Location = new Point(10, 25), AutoSize = true };
            grpPago.Controls.Add(lblImporte);
            txtImporte = new TextBox { Location = new Point(10, 45), Size = new Size(120, 24), Font = new Font("Segoe UI", 10f) };
            grpPago.Controls.Add(txtImporte);

            // Medio de pago
            lblMedioPago = new Label { Text = "Medio de pago *:", Location = new Point(160, 25), AutoSize = true };
            grpPago.Controls.Add(lblMedioPago);
            cmbMedioPago = new ComboBox
            {
                Location = new Point(160, 45),
                Size = new Size(180, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f)
            };
            cmbMedioPago.Items.AddRange(new[] { "Efectivo", "Tarjeta de Crédito" });
            cmbMedioPago.SelectedIndex = 0;
            grpPago.Controls.Add(cmbMedioPago);

            // Cantidad de cuotas
            lblCantCuotas = new Label { Text = "Cuotas:", Location = new Point(365, 25), AutoSize = true };
            grpPago.Controls.Add(lblCantCuotas);
            cmbCantCuotas = new ComboBox
            {
                Location = new Point(365, 45),
                Size = new Size(80, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                Enabled = false
            };
            cmbCantCuotas.Items.AddRange(new[] { "1", "3", "6" });
            cmbCantCuotas.SelectedIndex = 0;
            grpPago.Controls.Add(cmbCantCuotas);

            cmbMedioPago.SelectedIndexChanged += (s, e) =>
            {
                cmbCantCuotas.Enabled = cmbMedioPago.SelectedIndex == 1;
                if (!cmbCantCuotas.Enabled) cmbCantCuotas.SelectedIndex = 0;
            };

            // ── Estado / error ─────────────────────────────────────
            lblEstado = new Label
            {
                Text = "",
                Location = new Point(20, 355),
                Size = new Size(460, 22),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.DarkRed
            };
            this.Controls.Add(lblEstado);

            // ── Botones ────────────────────────────────────────────
            btnRegistrar = new Button
            {
                Text = "💾 Registrar Pago",
                Size = new Size(150, 36),
                Location = new Point(310, 385),
                BackColor = Color.FromArgb(30, 130, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnRegistrar.FlatAppearance.BorderSize = 0;
            btnRegistrar.Click += BtnRegistrar_Click;
            this.Controls.Add(btnRegistrar);

            btnCancelar = new Button
            {
                Text = "✖ Cancelar",
                Size = new Size(110, 36),
                Location = new Point(475, 385),
                BackColor = Color.FromArgb(190, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);
        }

        // ── Helpers visuales ─────────────────────────────────────────
        private void AgregarParLabel(Control padre, string etiqueta, int x, int y,
            out Label lbl, out TextBox txt, int ancho)
        {
            lbl = new Label { Text = etiqueta, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 8.5f) };
            txt = new TextBox { Location = new Point(x, y + 20), Size = new Size(ancho, 24), Font = new Font("Segoe UI", 9.5f) };
            padre.Controls.Add(lbl);
            padre.Controls.Add(txt);
        }

        // ── Buscar Socio ──────────────────────────────────────────────
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string busqueda = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(busqueda)) { MostrarError("Ingresá un término de búsqueda."); return; }

            try
            {
                var lista = _socioDAL.BuscarSocio(busqueda);
                if (lista.Count == 0)
                {
                    MostrarError("No se encontró ningún socio con ese criterio.");
                    LimpiarDatosSocio();
                    return;
                }
                if (lista.Count > 1)
                {
                    // Mostrar selector
                    using var sel = new FrmSeleccionarSocio(lista);
                    if (sel.ShowDialog(this) == DialogResult.OK)
                        CargarSocio(sel.SocioSeleccionado);
                    return;
                }
                CargarSocio(lista[0]);
            }
            catch (Exception ex) { MostrarError("Error: " + ex.Message); }
        }

        private void CargarSocio(Socio s)
        {
            _socioActual = s;
            txtNomSocio.Text = s.NombreCompleto;
            txtNumSocio.Text = s.NumeroSocio;
            txtEstadoSocio.Text = s.Estado.ToString();
            txtVencSocio.Text = s.FechaVencimientoCuota.HasValue
                ? s.FechaVencimientoCuota.Value.ToString("dd/MM/yyyy")
                : "Sin cuota registrada";

            txtEstadoSocio.ForeColor = s.Estado == EstadoSocio.Activo ? Color.DarkGreen : Color.DarkRed;
            btnRegistrar.Enabled = true;
            lblEstado.Text = "";
        }

        private void LimpiarDatosSocio()
        {
            _socioActual = null;
            txtNomSocio.Text = txtNumSocio.Text =
            txtEstadoSocio.Text = txtVencSocio.Text = "";
            btnRegistrar.Enabled = false;
        }

        // ── Registrar Pago ────────────────────────────────────────────
        private void BtnRegistrar_Click(object sender, EventArgs e)
        {
            if (_socioActual == null) { MostrarError("Primero buscá un socio."); return; }
            if (!decimal.TryParse(txtImporte.Text.Trim(), out decimal importe) || importe <= 0)
            {
                MostrarError("El importe debe ser un número mayor a cero.");
                txtImporte.Focus();
                return;
            }

            string medio = cmbMedioPago.SelectedIndex == 0 ? "Efectivo" : "TarjetaCredito";
            int cantCuotas = int.Parse(cmbCantCuotas.SelectedItem.ToString());

            try
            {
                btnRegistrar.Enabled = false;

                using var conn = ClubDeportivo.Data.Conexion.Instancia.ObtenerConexion();
                using var cmd = new MySql.Data.MySqlClient.MySqlCommand("sp_CobrarCuota", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_NumeroSocio", _socioActual.NumeroSocio);
                cmd.Parameters.AddWithValue("p_Importe", importe);
                cmd.Parameters.AddWithValue("p_MedioPago", medio);
                cmd.Parameters.AddWithValue("p_CantCuotas", cantCuotas);

                var pRes = new MySql.Data.MySqlClient.MySqlParameter("p_Resultado",
                    MySql.Data.MySqlClient.MySqlDbType.VarChar, 100)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                cmd.Parameters.Add(pRes);
                cmd.ExecuteNonQuery();

                string resultado = pRes.Value?.ToString() ?? "";

                if (resultado.StartsWith("OK"))
                {
                    MessageBox.Show($"✅ {resultado}", "Pago registrado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarDatosSocio();
                    txtBuscar.Clear();
                    txtImporte.Clear();
                    cmbMedioPago.SelectedIndex = 0;
                    cmbCantCuotas.SelectedIndex = 0;
                }
                else
                {
                    MostrarError(resultado);
                }
            }
            catch (Exception ex) { MostrarError("Error: " + ex.Message); }
            finally { btnRegistrar.Enabled = _socioActual != null; }
        }

        private void MostrarError(string msg)
        {
            lblEstado.ForeColor = Color.DarkRed;
            lblEstado.Text = msg;
        }

        private void InitializeComponent() { }
    }

    // ══════════════════════════════════════════════════════════════════
    //  Mini-formulario para elegir entre múltiples resultados
    // ══════════════════════════════════════════════════════════════════
    public class FrmSeleccionarSocio : Form
    {
        public Socio SocioSeleccionado { get; private set; }

        public FrmSeleccionarSocio(System.Collections.Generic.List<Socio> socios)
        {
            this.Text = "Seleccionar Socio";
            this.Size = new Size(520, 320);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9.5f);

            var lbl = new Label
            {
                Text = "Se encontraron varios socios. Seleccioná uno:",
                Location = new Point(15, 15),
                AutoSize = true
            };
            this.Controls.Add(lbl);

            var dgv = new DataGridView
            {
                Location = new Point(10, 40),
                Size = new Size(480, 190),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.Columns.Add("Num", "Nº Socio");
            dgv.Columns.Add("Ape", "Apellido");
            dgv.Columns.Add("Nom", "Nombre");
            dgv.Columns.Add("DNI", "DNI");
            dgv.Columns.Add("Est", "Estado");
            foreach (var s in socios)
                dgv.Rows.Add(s.NumeroSocio, s.Apellido, s.Nombre, s.DNI, s.Estado.ToString());
            this.Controls.Add(dgv);

            var btnSeleccionar = new Button
            {
                Text = "Seleccionar",
                Size = new Size(120, 32),
                Location = new Point(270, 245),
                BackColor = Color.FromArgb(30, 90, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnSeleccionar.FlatAppearance.BorderSize = 0;
            btnSeleccionar.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count > 0)
                {
                    int idx = dgv.SelectedRows[0].Index;
                    SocioSeleccionado = socios[idx];
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            };
            this.Controls.Add(btnSeleccionar);

            var btnCancel = new Button
            {
                Text = "Cancelar",
                Size = new Size(100, 32),
                Location = new Point(395, 245),
                BackColor = Color.FromArgb(190, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            this.Controls.Add(btnCancel);
        }
    }
}
