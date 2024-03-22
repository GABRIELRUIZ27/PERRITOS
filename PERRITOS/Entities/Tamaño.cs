using Perritos.Entities;

namespace PERRITOS.Entities
{
    public class Tamaño
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Perrito> Perritos { get; set; }
    }
}
