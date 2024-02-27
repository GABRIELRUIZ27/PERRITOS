using Perritos.DTOs;
using Perritos.Entities;
using Microsoft.EntityFrameworkCore;

namespace Perritos
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Perrito> Perritos { get; set; }
        public DbSet<Adoptado> Adoptados { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Discapacidad> Discapacidades { get; set; } 


    }
}