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
            ApplicationConfiguration.Initialize();

            // Mostrar el formulario de configuración de conexión
            using var frmConexion = new FrmConexion();
            if (frmConexion.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                // El usuario canceló o no pudo conectar → cerrar la aplicación
                return;
            }

            Application.Run(new FrmLogin());
        }
    }
}