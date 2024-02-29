namespace Perritos.Entities
{
    public class Perrito
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Genero Genero { get; set; }
        public bool Esterilizado { get; set; } 
        public string Edad { get; set; }
        public string Imagen { get; set; }
        public int? DiscapacidadId { get; set; }
        public Discapacidad Discapacidad { get; set; }
    }
}
