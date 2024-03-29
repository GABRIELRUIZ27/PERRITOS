﻿using Perritos.Entities;

namespace Perritos.DTOs
{
    public class AdoptadoDTO
    {
        public int? Id { get; set; }
        public PerritoDTO Perrito { get; set; }
        public DateTime FechaAdopcion { get; set; }
        public string? Imagen { get; set; }
        public string ImagenBase64 { get; set; }
    }
}
