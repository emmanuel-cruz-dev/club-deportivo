using System;
using ClubDeportivo.Data;
using ClubDeportivo.Models;
using MySql.Data.MySqlClient;


namespace ClubDeportivo.DAL
{
    /// Data Access Layer para operaciones relacionadas con Socios.
    /// Usa Stored Procedures definidos en ClubDeportivo_DB.sql
    public class SocioDAL
    {
        // ──────────────────────────────────────────────────────────────
        //  ALTA DE SOCIO
        // ──────────────────────────────────────────────────────────────

        /// Registra un nuevo socio llamando a sp_AltaSocio.
        /// Devuelve true si tuvo éxito; mensaje con resultado/error.
        public bool AltaSocio(
            Socio socio,
            decimal importeCuota,
            out string numeroSocio,
            out string numeroCarnet,
            out string mensaje)
        {
            numeroSocio = null;
            numeroCarnet = null;
            mensaje = string.Empty;

            try
            {
                using var conn = Conexion.Instancia.ObtenerConexion();
                using var cmd = new MySqlCommand("sp_AltaSocio", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };

                // Parámetros IN
                cmd.Parameters.AddWithValue("p_Nombre", socio.Nombre);
                cmd.Parameters.AddWithValue("p_Apellido", socio.Apellido);
                cmd.Parameters.AddWithValue("p_DNI", socio.DNI);
                cmd.Parameters.AddWithValue("p_Telefono", socio.Telefono ?? "");
                cmd.Parameters.AddWithValue("p_Email", socio.Email ?? "");
                cmd.Parameters.AddWithValue("p_Direccion", socio.Direccion ?? "");
                cmd.Parameters.AddWithValue("p_FechaNacimiento", socio.FechaNacimiento.Date);
                cmd.Parameters.AddWithValue("p_AptoFisico", socio.AptoFisicoPresentado ? 1 : 0);
                cmd.Parameters.AddWithValue("p_ImporteCuota", importeCuota);

                // Parámetros OUT
                var pNumSocio = new MySqlParameter("p_NumeroSocio", MySqlDbType.VarChar, 20)
                { Direction = System.Data.ParameterDirection.Output };
                var pNumCarnet = new MySqlParameter("p_NumeroCarnet", MySqlDbType.VarChar, 30)
                { Direction = System.Data.ParameterDirection.Output };
                var pResultado = new MySqlParameter("p_Resultado", MySqlDbType.VarChar, 100)
                { Direction = System.Data.ParameterDirection.Output };

                cmd.Parameters.Add(pNumSocio);
                cmd.Parameters.Add(pNumCarnet);
                cmd.Parameters.Add(pResultado);

                cmd.ExecuteNonQuery();

                mensaje = pResultado.Value?.ToString() ?? "";
                numeroSocio = pNumSocio.Value?.ToString() ?? "";
                numeroCarnet = pNumCarnet.Value?.ToString() ?? "";

                return mensaje == "OK";
            }
            catch (Exception ex)
            {
                mensaje = $"Error: {ex.Message}";
                return false;
            }
        }

        // ──────────────────────────────────────────────────────────────
        //  OBTENER TODOS LOS SOCIOS
        // ──────────────────────────────────────────────────────────────

        public List<Socio> ObtenerSocios()
        {
            var lista = new List<Socio>();
            try
            {
                using var conn = Conexion.Instancia.ObtenerConexion();
                using var cmd = new MySqlCommand("sp_ObtenerSocios", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                    lista.Add(MapearSocio(reader));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener socios: " + ex.Message, ex);
            }
            return lista;
        }

        // ──────────────────────────────────────────────────────────────
        //  BUSCAR SOCIO
        // ──────────────────────────────────────────────────────────────

        public List<Socio> BuscarSocio(string busqueda)
        {
            var lista = new List<Socio>();
            try
            {
                using var conn = Conexion.Instancia.ObtenerConexion();
                using var cmd = new MySqlCommand("sp_BuscarSocio", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_Busqueda", busqueda);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                    lista.Add(MapearSocio(reader));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar socio: " + ex.Message, ex);
            }
            return lista;
        }

        // ──────────────────────────────────────────────────────────────
        //  VERIFICAR SI DNI EXISTE
        // ──────────────────────────────────────────────────────────────

        public bool ExisteDNI(string dni)
        {
            try
            {
                using var conn = Conexion.Instancia.ObtenerConexion();
                using var cmd = new MySqlCommand("sp_ExisteDNI", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_DNI", dni);
                var resultado = cmd.ExecuteScalar();
                return Convert.ToInt32(resultado) > 0;
            }
            catch { return false; }
        }

        // ──────────────────────────────────────────────────────────────
        //  CONSULTAR VENCIMIENTOS
        // ──────────────────────────────────────────────────────────────

        public List<Socio> ConsultarVencimientos(int diasAnticipacion = 7)
        {
            var lista = new List<Socio>();
            try
            {
                using var conn = Conexion.Instancia.ObtenerConexion();
                using var cmd = new MySqlCommand("sp_ConsultarVencimientos", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_DiasAnticipacion", diasAnticipacion);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                    lista.Add(MapearSocio(reader));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar vencimientos: " + ex.Message, ex);
            }
            return lista;
        }

        // ──────────────────────────────────────────────────────────────
        //  MAPEO PRIVADO
        // ──────────────────────────────────────────────────────────────

        private static Socio MapearSocio(MySqlDataReader r)
        {
            var s = new Socio
            {
                IdSocio = r.GetInt32("IdSocio"),
                NumeroSocio = r.GetString("NumeroSocio"),
                Nombre = r.GetString("Nombre"),
                Apellido = r.GetString("Apellido"),
                DNI = r.GetString("DNI"),
                Telefono = r.IsDBNull(r.GetOrdinal("Telefono")) ? "" : r.GetString("Telefono"),
                Email = r.IsDBNull(r.GetOrdinal("Email")) ? "" : r.GetString("Email"),
                FechaAlta = r.GetDateTime("FechaAlta"),
                Estado = r.GetString("Estado") == "Activo"
                               ? EstadoSocio.Activo
                               : EstadoSocio.Suspendido,
                AptoFisicoPresentado = r.GetBoolean("AptoFisicoPresentado"),
                FechaVencimientoCuota = r.IsDBNull(r.GetOrdinal("FechaVencimientoCuota"))
                               ? (DateTime?)null
                               : r.GetDateTime("FechaVencimientoCuota")
            };
            return s;
        }
    }
}
