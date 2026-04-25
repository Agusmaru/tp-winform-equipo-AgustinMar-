using System;
using System.Collections.Generic;
using System.Text;

namespace dominio
{
    public interface IEntidadDescripcion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}
