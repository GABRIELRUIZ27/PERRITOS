using Perritos.Entities;

namespace Perritos.DTOs 
{
    public class PerritoDTO
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; }
        public GeneroDTO? Genero { get; set; }
        public bool? Esterilizado { get; set; }
        public string? Edad { get; set; }
        public string? Imagen {  get; set; }
        public string? ImagenBase64 { get; set; }
        public DiscapacidadDTO? Discapacidad { get; set; }
    }
}
