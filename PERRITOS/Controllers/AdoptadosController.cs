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
    [Route("api/adoptados")]
    [ApiController]
    public class AdoptadosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorImagenes almacenadorImagenes;
        private readonly string directorioAdoptados = "adoptados";

        public AdoptadosController(
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
        public async Task<ActionResult<AdoptadoDTO>> GetById(int id)
        {
            var adoptado = await context.Adoptados
                .Include(c => c.Perrito)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (adoptado == null)
            {
                return NotFound();
            }

            var adoptadoDTO = mapper.Map<AdoptadoDTO>(adoptado);

            if (!string.IsNullOrEmpty(adoptado.Imagen))
            {
                adoptadoDTO.ImagenBase64 = ObtenerBase64DesdeRutaImagen(adoptado.Imagen);
            }

            return Ok(adoptadoDTO);
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<AdoptadoDTO>>> GetAll()
        {
            try
            {
                var adoptados = await context.Adoptados
                    .Include(t => t.Perrito)
                    .ToListAsync();

                if (adoptados == null || adoptados.Count == 0)
                {
                    return NotFound();
                }

                var adoptadosDTO = mapper.Map<List<AdoptadoDTO>>(adoptados);

                foreach (var adoptadoDTO in adoptadosDTO)
                {
                    if (!string.IsNullOrEmpty(adoptadoDTO.Imagen))
                    {
                        adoptadoDTO.ImagenBase64 = ObtenerBase64DesdeRutaImagen(adoptadoDTO.Imagen);
                    }

                    // Verificar también la imagen en base64 para el objeto PerritoDTO
                    if (adoptadoDTO.Perrito != null && !string.IsNullOrEmpty(adoptadoDTO.Perrito.Imagen))
                    {
                        adoptadoDTO.Perrito.ImagenBase64 = ObtenerBase64DesdeRutaImagenPerritos(adoptadoDTO.Perrito.Imagen);
                    }
                }

                return Ok(adoptadosDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al obtener todos las adopciones.", details = ex.Message });
            }
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(AdoptadoDTO dto)
        {

            if (!string.IsNullOrEmpty(dto.ImagenBase64))
            {
                dto.Imagen = await almacenadorImagenes.GuardarImagen(dto.ImagenBase64, directorioAdoptados);
            }

            var adoptado = mapper.Map<Adoptado>(dto);
            adoptado.Perrito = await context.Perritos.SingleOrDefaultAsync(s => s.Id == dto.Perrito.Id);

            context.Adoptados.Add(adoptado);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var adoptado = await context.Adoptados.FindAsync(id);

            if (adoptado == null)
            {
                return NotFound();
            }


            context.Adoptados.Remove(adoptado);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, AdoptadoDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var adoptado = await context.Adoptados.FindAsync(id);

            if (adoptado == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(dto.ImagenBase64))
            {
                dto.Imagen = await almacenadorImagenes.GuardarImagen(dto.ImagenBase64, directorioAdoptados);
            }
            else
            {
                dto.Imagen = adoptado.Imagen;
            }

            mapper.Map(dto, adoptado);
            adoptado.Perrito = await context.Perritos.SingleOrDefaultAsync(c => c.Id == dto.Perrito.Id);

            context.Update(adoptado);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdopcionExists(id))
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

        private bool AdopcionExists(int id)
        {
            return context.Adoptados.Any(e => e.Id == id);
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
            // Encuentra la posición de "adoptados/" en la URL
            int indiceAdopcion = urlImagen.IndexOf("adoptados/");

            if (indiceAdopcion >= 0)
            {
                // Obtiene la parte de la URL que sigue después de "adoptados/"
                string rutaRelativa = urlImagen.Substring(indiceAdopcion + "adoptados/".Length);

                // Construye la ruta completa en el sistema de archivos
                string rutaCompleta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "adoptados", rutaRelativa);

                return rutaCompleta;
            }

            // Si no se encuentra "adoptados/" en la URL, devuelve null o maneja el error de acuerdo a tus necesidades
            return null;
        }

        private static string ObtenerBase64DesdeRutaImagenPerritos(string urlImagen)
        {
            try
            {
                // Obtén la ruta completa del archivo a partir de la URL de la imagen
                string rutaArchivo = ObtenerRutaArchivoDesdeUrlPerritos(urlImagen);

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
                    Console.WriteLine("La ruta de la imagen de perritos no es válida o el archivo no existe.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el base64 de la imagen de perritos: " + ex.Message);
                return null;
            }
        }

        private static string ObtenerRutaArchivoDesdeUrlPerritos(string urlImagen)
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