using ClubDeportivo.DAL;
using ClubDeportivo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubDeportivo.Forms
{
    /// Formulario para registrar un nuevo socio del club.
    /// Valida DNI único, apto físico obligatorio, y genera carnet automáticamente.
    public partial class FrmAltaSocio : Form
    {
        private readonly SocioDAL _socioDAL = new SocioDAL();

        // ── Controles ────────────────────────────────────────────────
        private Label lblTitulo;
        private GroupBox grpDatosPersonales;
        private GroupBox grpDatosIngreso;

        // Datos personales
        private Label lblNombre, lblApellido, lblDNI, lblTelefono;
        private Label lblEmail, lblDireccion, lblFechaNac;
        private TextBox txtNombre, txtApellido, txtDNI, txtTelefono;
        private TextBox txtEmail, txtDireccion;
        private DateTimePicker dtpFechaNac;

        // Datos ingreso
        private CheckBox chkAptoFisico;
        private Label lblImporteCuota, lblMedioPago, lblCantCuotas;
        private TextBox txtImporteCuota;
        private ComboBox cmbMedioPago, cmbCantCuotas;

        // Botones
        private Button btnGuardar, btnCancelar, btnLimpiar;
        private Label lblEstado;

        public FrmAltaSocio()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        // ─────────────────────────────────────────────────────────────
        private void ConfigurarFormulario()
        {
            this.Text = "Alta de Nuevo Socio";
            this.Size = new Size(700, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9.5f);

            // ── Título ─────────────────────────────────────────────
            lblTitulo = new Label
            {
                Text = "➕  Registrar Nuevo Socio",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 90, 160),
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            // ── GroupBox Datos Personales ──────────────────────────
            grpDatosPersonales = new GroupBox
            {
                Text = "Datos Personales",
                Location = new Point(15, 50),
                Size = new Size(655, 260),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 90, 160)
            };
            this.Controls.Add(grpDatosPersonales);

            // Fila 1: Nombre / Apellido
            AgregarLabel(grpDatosPersonales, "Nombre *", 15, 25, out lblNombre);
            AgregarLabel(grpDatosPersonales, "Apellido *", 340, 25, out lblApellido);
            txtNombre = AgregarTextBox(grpDatosPersonales, 15, 45, 280);
            txtApellido = AgregarTextBox(grpDatosPersonales, 340, 45, 295);

            // Fila 2: DNI / Teléfono
            AgregarLabel(grpDatosPersonales, "DNI *", 15, 80, out lblDNI);
            AgregarLabel(grpDatosPersonales, "Teléfono", 340, 80, out lblTelefono);
            txtDNI = AgregarTextBox(grpDatosPersonales, 15, 100, 200);
            txtTelefono = AgregarTextBox(grpDatosPersonales, 340, 100, 200);

            // Botón verificar DNI
            var btnVerDNI = new Button
            {
                Text = "Verificar",
                Size = new Size(85, 30),
                Location = new Point(225, 98),
                BackColor = Color.FromArgb(30, 90, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerDNI.FlatAppearance.BorderSize = 0;
            btnVerDNI.Click += BtnVerificarDNI_Click;
            grpDatosPersonales.Controls.Add(btnVerDNI);

            // Fila 3: Email / Fecha Nacimiento
            AgregarLabel(grpDatosPersonales, "Email", 15, 135, out lblEmail);
            AgregarLabel(grpDatosPersonales, "Fecha Nac. *", 340, 135, out lblFechaNac);
            txtEmail = AgregarTextBox(grpDatosPersonales, 15, 155, 295);

            dtpFechaNac = new DateTimePicker
            {
                Size = new Size(200, 30),
                Location = new Point(340, 155),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddYears(-18)
            };
            grpDatosPersonales.Controls.Add(dtpFechaNac);

            // Fila 4: Dirección
            AgregarLabel(grpDatosPersonales, "Dirección", 15, 190, out lblDireccion);
            txtDireccion = AgregarTextBox(grpDatosPersonales, 15, 210, 625);

            // ── GroupBox Datos de Ingreso ──────────────────────────s
            grpDatosIngreso = new GroupBox
            {
                Text = "Datos de Ingreso al Club",
                Location = new Point(15, 320),
                Size = new Size(655, 135),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 90, 160)
            };
            this.Controls.Add(grpDatosIngreso);

            // Apto físico
            chkAptoFisico = new CheckBox
            {
                Text = "Apto físico presentado *",
                Location = new Point(15, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f)
            };
            grpDatosIngreso.Controls.Add(chkAptoFisico);

            // Importe cuota
            AgregarLabel(grpDatosIngreso, "Importe cuota inicial ($)", 15, 65, out lblImporteCuota);
            txtImporteCuota = AgregarTextBox(grpDatosIngreso, 15, 85, 120);
            txtImporteCuota.Text = "0";

            // Medio de pago
            AgregarLabel(grpDatosIngreso, "Medio de pago", 220, 65, out lblMedioPago);
            cmbMedioPago = new ComboBox
            {
                Size = new Size(170, 30),
                Location = new Point(220, 85),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                IntegralHeight = false
            };
            cmbMedioPago.Items.AddRange(new[] { "Efectivo", "Tarjeta de Crédito" });
            cmbMedioPago.SelectedIndex = 0;
            grpDatosIngreso.Controls.Add(cmbMedioPago);

            // Cantidad de cuotas
            AgregarLabel(grpDatosIngreso, "Cuotas", 430, 65, out lblCantCuotas);
            cmbCantCuotas = new ComboBox
            {
                Size = new Size(70, 30),
                Location = new Point(430, 85),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                IntegralHeight = false
            };
            cmbCantCuotas.Items.AddRange(new[] { "1", "3", "6" });
            cmbCantCuotas.SelectedIndex = 0;
            grpDatosIngreso.Controls.Add(cmbCantCuotas);

            // Habilitar/deshabilitar cuotas según medio de pago
            cmbMedioPago.SelectedIndexChanged += (s, e) =>
            {
                cmbCantCuotas.Enabled = cmbMedioPago.SelectedIndex == 1;
                if (cmbMedioPago.SelectedIndex == 0) cmbCantCuotas.SelectedIndex = 0;
            };

            // ── Label de estado/error ─────────────────────────────
            lblEstado = new Label
            {
                Text = "",
                Location = new Point(20, 460),
                Size = new Size(500, 22),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.DarkRed
            };
            this.Controls.Add(lblEstado);

            // ── Botones ────────────────────────────────────────────
            btnGuardar = CrearBoton("Guardar", Color.FromArgb(30, 130, 70), new Point(360, 490));
            btnLimpiar = CrearBoton("Limpiar", Color.FromArgb(100, 110, 120), new Point(520, 490));
            btnCancelar = CrearBoton("Cancelar", Color.FromArgb(190, 50, 50), new Point(200, 490));

            btnGuardar.Click += BtnGuardar_Click;
            btnLimpiar.Click += (s, e) => LimpiarFormulario();
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnLimpiar);
            this.Controls.Add(btnCancelar);
        }

        // ── Helpers visuales ─────────────────────────────────────────
        private void AgregarLabel(Control padre, string texto, int x, int y, out Label lbl)
        {
            lbl = new Label
            {
                Text = texto,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(60, 60, 60)
            };
            padre.Controls.Add(lbl);
        }

        private TextBox AgregarTextBox(Control padre, int x, int y, int ancho)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(ancho, 30),
                Font = new Font("Segoe UI", 9.5f),
                AutoSize = false
            };
            padre.Controls.Add(txt);
            return txt;
        }

        private Button CrearBoton(string texto, Color color, Point location)
        {
            var btn = new Button
            {
                Text = texto,
                Size = new Size(150, 42),
                Location = location,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = false
            };

            btn.FlatAppearance.BorderSize = 0;

            return btn;
        }

        // ── Verificar DNI ─────────────────────────────────────────────
        private void BtnVerificarDNI_Click(object sender, EventArgs e)
        {
            string dni = txtDNI.Text.Trim();
            if (string.IsNullOrEmpty(dni)) { MostrarError("Ingresá un DNI para verificar."); return; }

            try
            {
                if (_socioDAL.ExisteDNI(dni))
                    MostrarError("⚠ El DNI ya se encuentra registrado en el sistema.", Color.DarkRed);
                else
                    MostrarError("✔ DNI disponible.", Color.DarkGreen);
            }
            catch (Exception ex) { MostrarError("Error al verificar DNI: " + ex.Message); }
        }

        // ── Guardar ───────────────────────────────────────────────────
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                btnGuardar.Enabled = false;

                decimal importe = decimal.TryParse(txtImporteCuota.Text, out decimal imp) ? imp : 0;

                var socio = new Socio
                {
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    DNI = txtDNI.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim(),
                    FechaNacimiento = dtpFechaNac.Value.Date,
                    AptoFisicoPresentado = chkAptoFisico.Checked
                };

                bool ok = _socioDAL.AltaSocio(
                    socio, importe,
                    out string numeroSocio,
                    out string numeroCarnet,
                    out string mensaje);

                if (ok)
                {
                    // Mostrar recibo/comprobante de alta
                    using var recibo = new FrmReciboAltaSocio(
                        $"{socio.Apellido}, {socio.Nombre}",
                        socio.DNI,
                        numeroSocio,
                        numeroCarnet,
                        DateTime.Now,
                        importe,
                        cmbMedioPago.SelectedItem?.ToString() ?? "Efectivo"
                    );
                    recibo.ShowDialog(this);

                    LimpiarFormulario();
                }
                else
                {
                    MostrarError(mensaje);
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error inesperado: " + ex.Message);
            }
            finally
            {
                btnGuardar.Enabled = true;
            }
        }

        // ── Validaciones ──────────────────────────────────────────────
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MostrarError("El nombre es obligatorio."); txtNombre.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            { MostrarError("El apellido es obligatorio."); txtApellido.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            { MostrarError("El DNI es obligatorio."); txtDNI.Focus(); return false; }
            if (!chkAptoFisico.Checked)
            { MostrarError("⚠ El apto físico es obligatorio para registrar un socio."); return false; }
            if (dtpFechaNac.Value.Date >= DateTime.Today)
            { MostrarError("La fecha de nacimiento no puede ser hoy o una fecha futura."); return false; }
            if (!decimal.TryParse(txtImporteCuota.Text, out _))
            { MostrarError("El importe debe ser un valor numérico."); txtImporteCuota.Focus(); return false; }

            lblEstado.Text = "";
            return true;
        }

        private void MostrarError(string msg, Color? color = null)
        {
            lblEstado.ForeColor = color ?? Color.DarkRed;
            lblEstado.Text = msg;
        }

        private void LimpiarFormulario()
        {
            txtNombre.Clear(); txtApellido.Clear();
            txtDNI.Clear(); txtTelefono.Clear();
            txtEmail.Clear(); txtDireccion.Clear();
            txtImporteCuota.Text = "0";
            dtpFechaNac.Value = DateTime.Today.AddYears(-18);
            chkAptoFisico.Checked = false;
            cmbMedioPago.SelectedIndex = 0;
            cmbCantCuotas.SelectedIndex = 0;
            lblEstado.Text = "";
            txtNombre.Focus();
        }

        private void InitializeComponent() { }
    }
}
