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

            return Ok(mapper.Map<PerritoDTO>(perrito));
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var perrito = await context.Perritos
                    .Include(t => t.Genero)
                    .Include(g => g.Discapacidad)
                    .ToListAsync();

                if (!perrito.Any())
                {
                    return NotFound();
                }

                return Ok(mapper.Map<List<PerritoDTO>>(perrito));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del candidatos ", details = ex.Message });
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

    }
}