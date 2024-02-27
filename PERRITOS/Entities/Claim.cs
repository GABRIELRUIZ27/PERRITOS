using System.ComponentModel.DataAnnotations;

namespace Perritos.Entities
{
    public class Claim
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public bool ClaimValue { get; set; }
        [Required]
        public Rol Rol { get; set; }
    }
}