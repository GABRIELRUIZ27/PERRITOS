using AutoMapper;
using Perritos.DTOs;
using Perritos.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perritos.Services;
using Perritos;

namespace simpatizantes_api.Controllers
{
    [Route("api/perritos")]
    [ApiController]
    public class PerritosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorImagenes almacenadorImagenes;
        private readonly string directorioPerritos = "perritos";

        public PerritosController( 
            ApplicationDbContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            IAlmacenadorImagenes almacenadorImagenes)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorImagenes = almacenadorImagenes;
        }

        [HttpGet("obtener-por-id/{id:int}")]
        public async Task<ActionResult<PerritoDTO>> GetById(int id)
        {
            var perrito = await context.Perritos
                .Include(c => c.Genero)
                .Include(g => g.Discapacidad)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (perrito == null)
            {
                return NotFound();
            }

            var perritoDTO = mapper.Map<PerritoDTO>(perrito);

            if (!string.IsNullOrEmpty(perrito.Imagen))
            {
                perritoDTO.ImagenBase64 = ObtenerBase64DesdeRutaImagen(perrito.Imagen);
            }

            return Ok(perritoDTO);
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<PerritoDTO>>> GetAll()
        {
            try
            {
                var perritos = await context.Perritos
                    .Include(t => t.Genero)
                    .Include(g => g.Discapacidad)
                    .ToListAsync();

                if (perritos == null || perritos.Count == 0)
                {
                    return NotFound();
                }

                var perritosDTO = mapper.Map<List<PerritoDTO>>(perritos);

                foreach (var perritoDTO in perritosDTO)
                {
                    if (!string.IsNullOrEmpty(perritoDTO.Imagen))
                    {
                        perritoDTO.ImagenBase64 = ObtenerBase64DesdeRutaImagen(perritoDTO.Imagen);
                    }
                }

                return Ok(perritosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al obtener todos los perritos.", details = ex.Message });
            }
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(PerritoDTO dto)
        {

            if (!string.IsNullOrEmpty(dto.ImagenBase64))
            {
                dto.Imagen = await almacenadorImagenes.GuardarImagen(dto.ImagenBase64, directorioPerritos);
            }

            var perro = mapper.Map<Perrito>(dto);
            perro.Genero = await context.Generos.SingleOrDefaultAsync(s => s.Id == dto.Genero.Id);

            if (dto.Discapacidad != null)
            {
                perro.Discapacidad = await context.Discapacidades.SingleOrDefaultAsync(p => p.Id == dto.Discapacidad.Id);
            }
            context.Perritos.Add(perro);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var perrito = await context.Perritos.FindAsync(id);

            if (perrito == null)
            {
                return NotFound();
            }

            var tieneDependencias = await context.Adoptados.AnyAsync(s => s.Id == id);

            if (tieneDependencias)
            {
                return StatusCode(502, new { error = "No se puede eliminar el simpatizante debido a dependencias existentes." });
            }

            context.Perritos.Remove(perrito);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, PerritoDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var perrito = await context.Perritos.FindAsync(id);

            if (perrito == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(dto.ImagenBase64))
            {
                dto.Imagen = await almacenadorImagenes.GuardarImagen(dto.ImagenBase64, directorioPerritos);
            }
            else
            {
                dto.Imagen = perrito.Imagen;
            }

            mapper.Map(dto, perrito);
            perrito.Genero = await context.Generos.SingleOrDefaultAsync(g => g.Id == dto.Genero.Id);

            if (dto.Discapacidad != null)
            {
                perrito.Discapacidad = await context.Discapacidades.SingleOrDefaultAsync(p => p.Id == dto.Discapacidad.Id);
            }

            context.Update(perrito);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerritosExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool PerritosExists(int id)
        {
            return context.Perritos.Any(e => e.Id == id);
        }
        private static string ObtenerBase64DesdeRutaImagen(string urlImagen)
        {
            try
            {
                // Obtén la ruta completa del archivo a partir de la URL de la imagen
                string rutaArchivo = ObtenerRutaArchivoDesdeUrl(urlImagen);

                // Verifica si la ruta del archivo es válida
                if (rutaArchivo != null && System.IO.File.Exists(rutaArchivo))
                {
                    // Lee el contenido del archivo como un array de bytes
                    byte[] imageArray = System.IO.File.ReadAllBytes(rutaArchivo);

                    // Convierte el array de bytes en una representación en base64
                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                    return base64ImageRepresentation;
                }
                else
                {
                    Console.WriteLine("La ruta de la imagen no es válida o el archivo no existe.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el base64 de la imagen: " + ex.Message);
                return null;
            }
        }

        private static string ObtenerRutaArchivoDesdeUrl(string urlImagen)
        {
            // Encuentra la posición de "perritos/" en la URL
            int indicePerritos = urlImagen.IndexOf("perritos/");

            if (indicePerritos >= 0)
            {
                // Obtiene la parte de la URL que sigue después de "perritos/"
                string rutaRelativa = urlImagen.Substring(indicePerritos + "perritos/".Length);

                // Construye la ruta completa en el sistema de archivos
                string rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "perritos", rutaRelativa);

                return rutaCompleta;
            }

            // Si no se encuentra "perritos/" en la URL, devuelve null o maneja el error de acuerdo a tus necesidades
            return null;
        }

    }
}