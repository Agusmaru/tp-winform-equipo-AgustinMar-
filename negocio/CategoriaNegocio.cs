using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;

namespace negocio
{
    public class CategoriaNegocio : CatalogoBaseNegocio<Categoria>
    {
        protected override string Nombretabla
        {
            get { return "CATEGORIAS"; }
        }

        protected override string ColumnaRefenrenciaArticulo
        {
            get { return "IdCategoria"; }
        }


    }
}
