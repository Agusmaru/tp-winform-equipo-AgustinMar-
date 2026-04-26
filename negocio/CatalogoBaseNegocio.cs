using dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
    public abstract class CatalogoBaseNegocio<T> where T : class, IEntidadDescripcion, new()
    {
        protected abstract string Nombretabla { get; }
        protected abstract string ColumnaRefenrenciaArticulo { get; }

        public List<T> Listar()
        {
            List<T> items = new List<T>();

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("SELECT Id, Descripcion FROM " + Nombretabla + "ORDER BY Descripcion");
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
                datos.setearConsulta("INSERT INTO " + Nombretabla + " (Descripcion) VALUES (@Descripcion)");
                datos.setearParametro("@Descripcion", descripcion.Trim());
                datos.ejecutarAccion();
            }

        }

        public void Modificar(int id, string descripcion)
        {
            ValidarDescripcion(descripcion);

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("UPDATE " + Nombretabla + " SET Descripcion = @Descripcion WHERE Id = @Id");
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
                datos.setearConsulta("DELETE FROM" + Nombretabla + " WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
        }

        private bool EstaEnUso(int id) 
        {
                using (AccesoDatos datos = new AccesoDatos())
                {
                    datos.setearConsulta("SELECT COUNT(*) FROM ARTICULOS WHERE " + ColumnaRefenrenciaArticulo + " = @Id");
                    datos.setearParametro("@Id", id);
                    return Convert.ToInt32(datos.ejecutarScalar()) > 0;
                }
        }

        private void ValidarDescripcion(string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion))
                throw new ArgumentException("La descripcion es obligatoria.");
        }


    }


   
}
