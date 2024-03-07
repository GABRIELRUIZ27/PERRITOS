namespace Perritos.Entities
{
    public class Discapacidad
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }
        public List<Perrito> Perritos { get; set; } 

    }
}
