using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace negocio
{
    public class AccesoDatos : IDisposable
    {
        private readonly SqlConnection conexion;
        private readonly SqlCommand comando;
        private SqlDataReader lector;

        public SqlDataReader Lector
        {
            get { return lector; }
        }

        public AccesoDatos()
        {
            string conexionConfigurada = ConfigurationManager.AppSettings["conexion-db"];
            if (string.IsNullOrWhiteSpace(conexionConfigurada))
                conexionConfigurada = "server=.\\SQLEXPRESS; database=CATALOGO_P3_DB; integrated security=true";

            conexion = new SqlConnection(conexionConfigurada);
            comando = new SqlCommand();
        }

        public void setearConsulta(string consulta)
        {
            comando.CommandType = CommandType.Text;
            comando.CommandText = consulta;
            comando.Parameters.Clear();
        }

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        public void ejecutarLectura()
        {
            comando.Connection = conexion;
            if (conexion.State != ConnectionState.Open)
                conexion.Open();

            lector = comando.ExecuteReader();
        }

        public int ejecutarAccion()
        {
            comando.Connection = conexion;
            if (conexion.State != ConnectionState.Open)
                conexion.Open();

            return comando.ExecuteNonQuery();
        }

        public object ejecutarScalar()
        {
            comando.Connection = conexion;
            if (conexion.State != ConnectionState.Open)
                conexion.Open();

            return comando.ExecuteScalar();
        }

        public void cerrarConexion()
        {
            if (lector != null && !lector.IsClosed)
                lector.Close();

            if (conexion.State == ConnectionState.Open)
                conexion.Close();
        }

        public void Dispose()
        {
            cerrarConexion();
            comando.Dispose();
            conexion.Dispose();
        }
    }
}
