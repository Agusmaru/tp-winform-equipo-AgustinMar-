using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace dominio
{
    public class Articulo
    {
        public Articulo() { 
            Imagenes = new List<ImagenArticulo>(); 
            Marca = new Marca();
            Categoria = new Categoria();
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Marca Marca { get; set; }
        public Categoria Categoria { get; set; }
        public decimal Precio { get; set; }
        public List<ImagenArticulo> Imagenes { get; set; }

        public string MarcaDescripcion { 
            get { return Marca != null ? Marca.Descripcion : string.Empty; } 
        }

        public string CategoriaDescripcion {
            get { return Categoria != null ? Categoria.Descripcion : string.Empty; } 
        }

        public string ImagenPrincipal {
            get { return Imagenes.FirstOrDefault() != null ? Imagenes.First().ImagenUrl : string.Empty; } 
        }




    }
}
