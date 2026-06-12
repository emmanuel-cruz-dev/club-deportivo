using System;
using System.Collections.Generic;
using System.Text;
using ClubDeportivo.DAL;
using ClubDeportivo.Models;
using MySql.Data.MySqlClient;
using ClubDeportivo.Data;

namespace ClubDeportivo.Forms
{    
    /// Formulario para registrar un No Socio del club.
    /// Sin carnet ni cuotas; solo datos personales y observaciones.    
    public partial class FrmAltaNoSocio : Form
    {
        private readonly SocioDAL _socioDAL = new SocioDAL();

        private Label lblTitulo;
        private GroupBox grpDatos;

        private Label lblNombre, lblApellido, lblDNI, lblTelefono;
        private Label lblEmail, lblDireccion, lblFechaNac, lblObservaciones;
        private TextBox txtNombre, txtApellido, txtDNI, txtTelefono;
        private TextBox txtEmail, txtDireccion, txtObservaciones;
        private DateTimePicker dtpFechaNac;

        private Button btnGuardar, btnCancelar, btnLimpiar;
        private Label lblEstado;

        public FrmAltaNoSocio()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.Text = "Registro de No Socio";
            this.Size = new Size(660, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Font = new Font("Segoe UI", 9.5f);

            lblTitulo = new Label
            {
                Text = "🙋  Registrar No Socio",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 120, 130),
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitulo);

            grpDatos = new GroupBox
            {
                Text = "Datos Personales",
                Location = new Point(15, 50),
                Size = new Size(616, 310),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 120, 130)
            };
            this.Controls.Add(grpDatos);

            // Fila 1: Nombre / Apellido
            Lbl(grpDatos, "Nombre *", 15, 25, out lblNombre);
            Lbl(grpDatos, "Apellido *", 330, 25, out lblApellido);
            txtNombre = Txt(grpDatos, 15, 45, 280);
            txtApellido = Txt(grpDatos, 330, 45, 270);

            // Fila 2: DNI / Teléfono
            Lbl(grpDatos, "DNI *", 15, 80, out lblDNI);
            Lbl(grpDatos, "Teléfono", 330, 80, out lblTelefono);
            txtDNI = Txt(grpDatos, 15, 100, 200);
            txtTelefono = Txt(grpDatos, 330, 100, 200);

            // Botón verificar DNI
            var btnVer = new Button
            {
                Text = "Verificar",
                Size = new Size(72, 34),
                Location = new Point(223, 100),
                BackColor = Color.FromArgb(60, 120, 130),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand
            };
            btnVer.FlatAppearance.BorderSize = 0;
            btnVer.Click += (s, e) =>
            {
                string d = txtDNI.Text.Trim();
                if (string.IsNullOrEmpty(d)) return;
                MostrarEstado(
                    _socioDAL.ExisteDNI(d)
                        ? "⚠ El DNI ya está registrado en el sistema."
                        : "✔ DNI disponible.",
                    _socioDAL.ExisteDNI(d) ? Color.DarkRed : Color.DarkGreen);
            };
            grpDatos.Controls.Add(btnVer);

            // Fila 3: Email / Fecha Nac.
            Lbl(grpDatos, "Email", 15, 135, out lblEmail);
            Lbl(grpDatos, "Fecha Nac. *", 330, 135, out lblFechaNac);
            txtEmail = Txt(grpDatos, 15, 155, 280);
            dtpFechaNac = new DateTimePicker
            {
                Size = new Size(200, 24),
                Location = new Point(330, 155),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddYears(-18)
            };
            grpDatos.Controls.Add(dtpFechaNac);

            // Fila 4: Dirección
            Lbl(grpDatos, "Dirección", 15, 190, out lblDireccion);
            txtDireccion = Txt(grpDatos, 15, 210, 590);

            // Fila 5: Observaciones
            Lbl(grpDatos, "Observaciones", 15, 245, out lblObservaciones);
            txtObservaciones = new TextBox
            {
                Location = new Point(15, 265),
                Size = new Size(590, 30),
                Font = new Font("Segoe UI", 9.5f),
                Multiline = true
            };
            grpDatos.Controls.Add(txtObservaciones);

            // Estado
            lblEstado = new Label
            {
                Text = "",
                Location = new Point(20, 370),
                Size = new Size(480, 22),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.DarkRed
            };
            this.Controls.Add(lblEstado);

            // Botones
            btnCancelar = Boton("✖  Cancelar", Color.FromArgb(190, 50, 50), new Point(205, 393));
            btnGuardar = Boton("💾  Guardar", Color.FromArgb(30, 130, 70), new Point(350, 393));
            btnLimpiar = Boton("🔄  Limpiar", Color.FromArgb(90, 100, 110), new Point(495, 393));

            btnGuardar.Click += BtnGuardar_Click;
            btnLimpiar.Click += (s, e) => Limpiar();
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.Add(btnGuardar);
            this.Controls.Add(btnLimpiar);
            this.Controls.Add(btnCancelar);
        }

        // ── Guardar ───────────────────────────────────────────────────
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!Validar()) return;

            try
            {
                btnGuardar.Enabled = false;

                using var conn = Conexion.Instancia.ObtenerConexion();
                using var cmd = new MySqlCommand("sp_AltaNoSocio", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_Nombre", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("p_Apellido", txtApellido.Text.Trim());
                cmd.Parameters.AddWithValue("p_DNI", txtDNI.Text.Trim());
                cmd.Parameters.AddWithValue("p_Telefono", txtTelefono.Text.Trim());
                cmd.Parameters.AddWithValue("p_Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("p_Direccion", txtDireccion.Text.Trim());
                cmd.Parameters.AddWithValue("p_FechaNacimiento", dtpFechaNac.Value.Date);
                cmd.Parameters.AddWithValue("p_Observaciones", txtObservaciones.Text.Trim());

                var pRes = new MySqlParameter("p_Resultado", MySqlDbType.VarChar, 100)
                { Direction = System.Data.ParameterDirection.Output };
                cmd.Parameters.Add(pRes);
                cmd.ExecuteNonQuery();

                string resultado = pRes.Value?.ToString() ?? "";
                if (resultado == "OK")
                {
                    MessageBox.Show("✅ No socio registrado exitosamente.", "Alta exitosa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Limpiar();
                }
                else
                {
                    MostrarEstado(resultado, Color.DarkRed);
                }
            }
            catch (Exception ex) { MostrarEstado("Error: " + ex.Message, Color.DarkRed); }
            finally { btnGuardar.Enabled = true; }
        }

        private bool Validar()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            { MostrarEstado("El nombre es obligatorio."); txtNombre.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            { MostrarEstado("El apellido es obligatorio."); txtApellido.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtDNI.Text))
            { MostrarEstado("El DNI es obligatorio."); txtDNI.Focus(); return false; }
            return true;
        }

        private void Limpiar()
        {
            txtNombre.Clear(); txtApellido.Clear(); txtDNI.Clear();
            txtTelefono.Clear(); txtEmail.Clear(); txtDireccion.Clear();
            txtObservaciones.Clear();
            dtpFechaNac.Value = DateTime.Today.AddYears(-18);
            lblEstado.Text = "";
            txtNombre.Focus();
        }

        private void MostrarEstado(string msg, Color? color = null)
        {
            lblEstado.ForeColor = color ?? Color.DarkRed;
            lblEstado.Text = msg;
        }

        // ── Helpers ───────────────────────────────────────────────────
        private void Lbl(Control p, string t, int x, int y, out Label l)
        {
            l = new Label { Text = t, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(60, 60, 60) };
            p.Controls.Add(l);
        }
        private TextBox Txt(Control p, int x, int y, int w)
        {
            var t = new TextBox { Location = new Point(x, y), Size = new Size(w, 24), Font = new Font("Segoe UI", 9.5f) };
            p.Controls.Add(t); return t;
        }
        private Button Boton(string texto, Color color, Point loc)
        {
            var b = new Button { Text = texto, Size = new Size(134, 34), Location = loc, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9f, FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; return b;
        }

        private void InitializeComponent() { }
    }
}
