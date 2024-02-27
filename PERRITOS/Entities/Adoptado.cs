namespace Perritos.Entities
{
    public class Adoptado 
    {
        public int Id { get; set; }
        public Perrito Perrito { get; set; }
        public DateTime FechaAdopcion { get; set; }
        public string Foto { get; set; }
    }
}
