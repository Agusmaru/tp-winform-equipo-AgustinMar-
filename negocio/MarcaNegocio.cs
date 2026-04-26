using dominio;

namespace negocio
{
    public class MarcaNegocio : CatalogoBaseNegocio<Marca>
    {
        protected override string NombreTabla
        {
            get { return "MARCAS"; }
        }

        protected override string ColumnaReferenciaArticulo
        {
            get { return "IdMarca"; }
        }
    }
}
