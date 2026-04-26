using System;
using System.Collections.Generic;
using dominio;

namespace negocio
{
    public abstract class CatalogoBaseNegocio<T> where T : class, IEntidadDescripcion, new()
    {
        protected abstract string NombreTabla { get; }
        protected abstract string ColumnaReferenciaArticulo { get; }

        public List<T> Listar()
        {
            List<T> items = new List<T>();

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM " + NombreTabla + " ORDER BY Descripcion");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    items.Add(new T
                    {
                        Id = (int)datos.Lector["Id"],
                        Descripcion = datos.Lector["Descripcion"] != DBNull.Value ? datos.Lector["Descripcion"].ToString() : string.Empty
                    });
                }
            }

            return items;
        }

        public void Agregar(string descripcion)
        {
            ValidarDescripcion(descripcion);

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("INSERT INTO " + NombreTabla + " (Descripcion) VALUES (@Descripcion)");
                datos.setearParametro("@Descripcion", descripcion.Trim());
                datos.ejecutarAccion();
            }
        }

        public void Modificar(int id, string descripcion)
        {
            ValidarDescripcion(descripcion);

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("UPDATE " + NombreTabla + " SET Descripcion = @Descripcion WHERE Id = @Id");
                datos.setearParametro("@Descripcion", descripcion.Trim());
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
        }

        public void Eliminar(int id)
        {
            if (EstaEnUso(id))
                throw new InvalidOperationException("No se puede eliminar porque hay articulos asociados.");

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("DELETE FROM " + NombreTabla + " WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
        }

        private bool EstaEnUso(int id)
        {
            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("SELECT COUNT(*) FROM ARTICULOS WHERE " + ColumnaReferenciaArticulo + " = @Id");
                datos.setearParametro("@Id", id);
                return Convert.ToInt32(datos.ejecutarScalar()) > 0;
            }
        }

        private void ValidarDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripcion es obligatoria.");
        }
    }
}
