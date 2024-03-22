using PERRITOS.DTOs;
using PERRITOS.Entities;

namespace Perritos.Entities
{
    public class Perrito
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Genero Genero { get; set; }
        public bool Esterilizado { get; set; } 
        public string Imagen { get; set; }
        public int? DiscapacidadId { get; set; }
        public Discapacidad Discapacidad { get; set; }
        public Tamaño Tamaño { get; set; }
        public Edad Edad { get; set; } 
    }
}
