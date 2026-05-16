using ClubDeportivo.Data;
using ClubDeportivo.Forms;

namespace ClubDeportivo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            if (!Conexion.Instancia.ProbarConexion(out string msg))
            {
                MessageBox.Show(
                    $"No se pudo conectar a la base de datos.\n\n{msg}\n\n" +
                    "Verificá que MySQL esté corriendo y que los datos en Conexion.cs sean correctos.",
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new FrmLogin());
        }
    }
}