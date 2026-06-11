using System;
using MySql.Data.MySqlClient;

namespace ClubDeportivo.Data
{
    public class Conexion
    {
        // ── Configuración dinámica (ingresada por teclado al iniciar la app) ──────
        private static string _connectionString = string.Empty;
        private static bool _configurada = false;

        // Instancia Singleton
        private static Conexion _instancia;
        public static Conexion Instancia => _instancia ??= new Conexion();

        private Conexion() { }

        /// <summary>
        /// Debe llamarse UNA vez al iniciar la aplicación con los datos
        /// ingresados por el usuario (T_SERVER, T_USER, T_PASSWORD, T_PORT).
        /// </summary>
        public static void Configurar(string tServer, string tUser, string tPassword, int tPort)
        {
            const string DATABASE = "ClubDeportivo";
            _connectionString =
                $"Server={tServer};Port={tPort};Database={DATABASE};" +
                $"Uid={tUser};Pwd={tPassword};CharSet=utf8mb4;";
            _configurada = true;
            // Resetear el singleton para que tome la nueva cadena
            _instancia = null;
        }

        /// Devuelve una nueva conexión abierta.
        public MySqlConnection ObtenerConexion()
        {
            if (!_configurada)
                throw new InvalidOperationException(
                    "La conexión no fue configurada. Llamá a Conexion.Configurar() antes de usarla.");

            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        /// Verifica que la base de datos sea accesible.
        public bool ProbarConexion(out string mensaje)
        {
            try
            {
                using var conn = ObtenerConexion();
                mensaje = "Conexión exitosa.";
                return true;
            }
            catch (Exception ex)
            {
                mensaje = $"Error al conectar: {ex.Message}";
                return false;
            }
        }
    }
}
