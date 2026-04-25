using System;
using System.Collections.Generic;
using System.Text;

namespace dominio
{
    public class Categoria : IEntidadDescripcion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}
