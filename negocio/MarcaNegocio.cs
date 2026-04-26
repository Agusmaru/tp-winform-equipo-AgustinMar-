using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dominio;

namespace negocio
{
    public class MarcaNegocio : CatalogoBaseNegocio<Marca>
    {
        protected override string Nombretabla
        {
            get { return "MARCAS"; }
        }

        protected override string ColumnaRefenrenciaArticulo
        {
            get { return "IdMarca"; }
        }
    }
}
