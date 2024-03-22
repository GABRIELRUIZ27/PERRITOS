using Perritos.Entities;

namespace PERRITOS.Entities
{
    public class Edad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Perrito> Perritos { get; set; }
    }
}
