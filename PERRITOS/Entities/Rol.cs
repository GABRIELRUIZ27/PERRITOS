using Perritos.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Perritos.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string NombreRol { get; set; }
        public List<Usuario> Usuarios { get; set; }
        public List<Claim> Claims { get; set; }
    }
}