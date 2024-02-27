using System.ComponentModel.DataAnnotations;

namespace Perritos.Entities
{
    public class Usuario
    {

        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public bool Estatus { get; set; }

        [Required]
        public Rol Rol { get; set; }
    }
}