using System;
using MySql.Data.MySqlClient;
using ClubDeportivo.Data;

namespace ClubDeportivo.DAL
{
    public class UsuarioDAL
    {        
        /// Valida credenciales contra la tabla Usuarios.
        /// Devuelve el nombre completo si es válido, null si no.        
        public string ValidarLogin(string usuario, string contrasena)
        {
            try
            {
                using var conn = Conexion.Instancia.ObtenerConexion();
                using var cmd = new MySqlCommand("sp_ValidarLogin", conn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("p_NombreUsuario", usuario);
                cmd.Parameters.AddWithValue("p_Contrasena", contrasena);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    return reader.GetString("NombreCompleto");

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar login: " + ex.Message, ex);
            }
        }
    }
}
