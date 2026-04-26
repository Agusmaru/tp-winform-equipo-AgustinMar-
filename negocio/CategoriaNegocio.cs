using dominio;

namespace negocio
{
    public class CategoriaNegocio : CatalogoBaseNegocio<Categoria>
    {
        protected override string NombreTabla
        {
            get { return "CATEGORIAS"; }
        }

        protected override string ColumnaReferenciaArticulo
        {
            get { return "IdCategoria"; }
        }
    }
}
