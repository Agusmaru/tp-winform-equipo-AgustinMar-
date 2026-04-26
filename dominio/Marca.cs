namespace dominio
{
    public class Marca : IEntidadDescripcion
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public override string ToString()
        {
            return Descripcion;
        }
    }
}
