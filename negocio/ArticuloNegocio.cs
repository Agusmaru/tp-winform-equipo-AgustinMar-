using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using dominio;

namespace negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> Listar()
        {
            List<Articulo> articulos = new List<Articulo>();

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta(@"
SELECT
    A.Id,
    A.Codigo,
    A.Nombre,
    A.Descripcion,
    A.Precio,
    M.Id AS MarcaId,
    M.Descripcion AS MarcaDescripcion,
    C.Id AS CategoriaId,
    C.Descripcion AS CategoriaDescripcion,
    IMG.ImagenUrl AS ImagenPrincipal
FROM ARTICULOS A
LEFT JOIN MARCAS M ON M.Id = A.IdMarca
LEFT JOIN CATEGORIAS C ON C.Id = A.IdCategoria
OUTER APPLY (
    SELECT TOP 1 ImagenUrl
    FROM IMAGENES I
    WHERE I.IdArticulo = A.Id
    ORDER BY I.Id
) IMG
ORDER BY A.Nombre");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                    articulos.Add(MapearArticulo(datos.Lector));
            }

            return articulos;
        }

        public Articulo ObtenerPorId(int id)
        {
            Articulo articulo = null;

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta(@"
SELECT
    A.Id,
    A.Codigo,
    A.Nombre,
    A.Descripcion,
    A.Precio,
    M.Id AS MarcaId,
    M.Descripcion AS MarcaDescripcion,
    C.Id AS CategoriaId,
    C.Descripcion AS CategoriaDescripcion
FROM ARTICULOS A
LEFT JOIN MARCAS M ON M.Id = A.IdMarca
LEFT JOIN CATEGORIAS C ON C.Id = A.IdCategoria
WHERE A.Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                    articulo = MapearArticulo(datos.Lector);
            }

            if (articulo != null)
                articulo.Imagenes = ListarImagenes(id);

            return articulo;
        }

        public void Agregar(Articulo articulo)
        {
            Validar(articulo);

            int idGenerado;
            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta(@"
INSERT INTO ARTICULOS (Codigo, Nombre, Descripcion, IdMarca, IdCategoria, Precio)
VALUES (@Codigo, @Nombre, @Descripcion, @IdMarca, @IdCategoria, @Precio);
SELECT SCOPE_IDENTITY();");
                CargarParametrosArticulo(datos, articulo);
                idGenerado = Convert.ToInt32(datos.ejecutarScalar());
            }

            GuardarImagenes(idGenerado, articulo.Imagenes);
        }

        public void Modificar(Articulo articulo)
        {
            Validar(articulo);

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta(@"
UPDATE ARTICULOS
SET Codigo = @Codigo,
    Nombre = @Nombre,
    Descripcion = @Descripcion,
    IdMarca = @IdMarca,
    IdCategoria = @IdCategoria,
    Precio = @Precio
WHERE Id = @Id");
                CargarParametrosArticulo(datos, articulo);
                datos.setearParametro("@Id", articulo.Id);
                datos.ejecutarAccion();
            }

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("DELETE FROM IMAGENES WHERE IdArticulo = @IdArticulo");
                datos.setearParametro("@IdArticulo", articulo.Id);
                datos.ejecutarAccion();
            }

            GuardarImagenes(articulo.Id, articulo.Imagenes);
        }

        public void Eliminar(int id)
        {
            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("DELETE FROM IMAGENES WHERE IdArticulo = @IdArticulo");
                datos.setearParametro("@IdArticulo", id);
                datos.ejecutarAccion();
            }

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("DELETE FROM ARTICULOS WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
        }

        public List<Articulo> Filtrar(string campo, string criterio, string valor)
        {
            List<Articulo> articulos = new List<Articulo>();
            string columna = ObtenerColumna(campo);
            string condicion;
            object valorParametro;

            if (campo == "Precio")
            {
                decimal precio;
                if (!decimal.TryParse(valor, out precio))
                    throw new ArgumentException("Para filtrar por precio debe ingresar un numero valido.");

                condicion = ObtenerCondicionNumerica(criterio, columna);
                valorParametro = precio;
            }
            else
            {
                condicion = ObtenerCondicionTexto(criterio, columna, valor, out valorParametro);
            }

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta(@"
SELECT
    A.Id,
    A.Codigo,
    A.Nombre,
    A.Descripcion,
    A.Precio,
    M.Id AS MarcaId,
    M.Descripcion AS MarcaDescripcion,
    C.Id AS CategoriaId,
    C.Descripcion AS CategoriaDescripcion,
    IMG.ImagenUrl AS ImagenPrincipal
FROM ARTICULOS A
LEFT JOIN MARCAS M ON M.Id = A.IdMarca
LEFT JOIN CATEGORIAS C ON C.Id = A.IdCategoria
OUTER APPLY (
    SELECT TOP 1 ImagenUrl
    FROM IMAGENES I
    WHERE I.IdArticulo = A.Id
    ORDER BY I.Id
) IMG
WHERE " + condicion + @"
ORDER BY A.Nombre");
                datos.setearParametro("@Valor", valorParametro);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                    articulos.Add(MapearArticulo(datos.Lector));
            }

            return articulos;
        }

        public List<ImagenArticulo> ListarImagenes(int idArticulo)
        {
            List<ImagenArticulo> imagenes = new List<ImagenArticulo>();

            using (AccesoDatos datos = new AccesoDatos())
            {
                datos.setearConsulta("SELECT Id, IdArticulo, ImagenUrl FROM IMAGENES WHERE IdArticulo = @IdArticulo ORDER BY Id");
                datos.setearParametro("@IdArticulo", idArticulo);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    imagenes.Add(new ImagenArticulo
                    {
                        Id = (int)datos.Lector["Id"],
                        IdArticulo = (int)datos.Lector["IdArticulo"],
                        ImagenUrl = datos.Lector["ImagenUrl"].ToString()
                    });
                }
            }

            return imagenes;
        }

        private void GuardarImagenes(int idArticulo, List<ImagenArticulo> imagenes)
        {
            foreach (ImagenArticulo imagen in imagenes)
            {
                using (AccesoDatos datos = new AccesoDatos())
                {
                    datos.setearConsulta("INSERT INTO IMAGENES (IdArticulo, ImagenUrl) VALUES (@IdArticulo, @ImagenUrl)");
                    datos.setearParametro("@IdArticulo", idArticulo);
                    datos.setearParametro("@ImagenUrl", imagen.ImagenUrl.Trim());
                    datos.ejecutarAccion();
                }
            }
        }

        private void CargarParametrosArticulo(AccesoDatos datos, Articulo articulo)
        {
            datos.setearParametro("@Codigo", articulo.Codigo.Trim());
            datos.setearParametro("@Nombre", articulo.Nombre.Trim());
            datos.setearParametro("@Descripcion", articulo.Descripcion.Trim());
            datos.setearParametro("@IdMarca", articulo.Marca.Id);
            datos.setearParametro("@IdCategoria", articulo.Categoria.Id);
            datos.setearParametro("@Precio", articulo.Precio);
        }

        private Articulo MapearArticulo(SqlDataReader lector)
        {
            Articulo articulo = new Articulo
            {
                Id = (int)lector["Id"],
                Codigo = lector["Codigo"] != DBNull.Value ? lector["Codigo"].ToString() : string.Empty,
                Nombre = lector["Nombre"] != DBNull.Value ? lector["Nombre"].ToString() : string.Empty,
                Descripcion = lector["Descripcion"] != DBNull.Value ? lector["Descripcion"].ToString() : string.Empty,
                Precio = lector["Precio"] != DBNull.Value ? Convert.ToDecimal(lector["Precio"]) : 0
            };

            articulo.Marca = new Marca
            {
                Id = lector["MarcaId"] != DBNull.Value ? Convert.ToInt32(lector["MarcaId"]) : 0,
                Descripcion = lector["MarcaDescripcion"] != DBNull.Value ? lector["MarcaDescripcion"].ToString() : "Sin marca"
            };

            articulo.Categoria = new Categoria
            {
                Id = lector["CategoriaId"] != DBNull.Value ? Convert.ToInt32(lector["CategoriaId"]) : 0,
                Descripcion = lector["CategoriaDescripcion"] != DBNull.Value ? lector["CategoriaDescripcion"].ToString() : "Sin categoria"
            };

            if (TieneColumna(lector, "ImagenPrincipal") && lector["ImagenPrincipal"] != DBNull.Value)
            {
                articulo.Imagenes.Add(new ImagenArticulo
                {
                    IdArticulo = articulo.Id,
                    ImagenUrl = lector["ImagenPrincipal"].ToString()
                });
            }

            return articulo;
        }

        private bool TieneColumna(SqlDataReader lector, string nombre)
        {
            for (int i = 0; i < lector.FieldCount; i++)
            {
                if (lector.GetName(i) == nombre)
                    return true;
            }

            return false;
        }

        private void Validar(Articulo articulo)
        {
            if (articulo == null)
                throw new ArgumentException("No se recibio el articulo a guardar.");
            if (string.IsNullOrWhiteSpace(articulo.Codigo))
                throw new ArgumentException("El codigo es obligatorio.");
            if (string.IsNullOrWhiteSpace(articulo.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(articulo.Descripcion))
                throw new ArgumentException("La descripcion es obligatoria.");
            if (articulo.Marca == null || articulo.Marca.Id <= 0)
                throw new ArgumentException("Debe seleccionar una marca.");
            if (articulo.Categoria == null || articulo.Categoria.Id <= 0)
                throw new ArgumentException("Debe seleccionar una categoria.");
            if (articulo.Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero.");
            if (articulo.Imagenes == null || articulo.Imagenes.Count == 0)
                throw new ArgumentException("Debe cargar al menos una imagen.");

            foreach (ImagenArticulo imagen in articulo.Imagenes)
            {
                if (string.IsNullOrWhiteSpace(imagen.ImagenUrl))
                    throw new ArgumentException("Las imagenes no pueden estar vacias.");
            }
        }

        private string ObtenerColumna(string campo)
        {
            switch (campo)
            {
                case "Codigo":
                    return "A.Codigo";
                case "Nombre":
                    return "A.Nombre";
                case "Marca":
                    return "M.Descripcion";
                case "Categoria":
                    return "C.Descripcion";
                case "Precio":
                    return "A.Precio";
                default:
                    throw new ArgumentException("El campo seleccionado no es valido.");
            }
        }

        private string ObtenerCondicionNumerica(string criterio, string columna)
        {
            switch (criterio)
            {
                case "Mayor a":
                    return columna + " > @Valor";
                case "Menor a":
                    return columna + " < @Valor";
                case "Igual a":
                    return columna + " = @Valor";
                default:
                    throw new ArgumentException("El criterio seleccionado no es valido.");
            }
        }

        private string ObtenerCondicionTexto(string criterio, string columna, string valor, out object valorParametro)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("Debe ingresar un valor para filtrar.");

            switch (criterio)
            {
                case "Comienza con":
                    valorParametro = valor.Trim() + "%";
                    return columna + " LIKE @Valor";
                case "Termina con":
                    valorParametro = "%" + valor.Trim();
                    return columna + " LIKE @Valor";
                case "Contiene":
                    valorParametro = "%" + valor.Trim() + "%";
                    return columna + " LIKE @Valor";
                default:
                    throw new ArgumentException("El criterio seleccionado no es valido.");
            }
        }
    }
}
