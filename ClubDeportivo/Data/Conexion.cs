using System;
using MySql.Data.MySqlClient;

namespace ClubDeportivo.Data
{
    public class Conexion
    {
        // ── Configuración de la conexión a la base de datos ──────────────────────────────────────────
        private const string SERVER = "localhost";
        private const string DATABASE = "ClubDeportivo";
        private const string USER = "root";
        private const string PASSWORD = "root";
        private const int PORT = 3306;
        // ─────────────────────────────────────────────────────────────────────

        private static readonly string _connectionString =
            $"Server={SERVER};Port={PORT};Database={DATABASE};" +
            $"Uid={USER};Pwd={PASSWORD};CharSet=utf8mb4;";

        // Instancia Singleton
        private static Conexion _instancia;
        public static Conexion Instancia => _instancia ??= new Conexion();

        private Conexion() { }

        /// Devuelve una nueva conexión abierta.
        public MySqlConnection ObtenerConexion()
        {
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
