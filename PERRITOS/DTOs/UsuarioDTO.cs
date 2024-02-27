using System.ComponentModel.DataAnnotations;

namespace Perritos.DTOs
{
    public class UsuarioDTO
    {
        public int? Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public bool Estatus { get; set; }
        [Required]
        public RolDTO Rol { get; set; }


    }
}
