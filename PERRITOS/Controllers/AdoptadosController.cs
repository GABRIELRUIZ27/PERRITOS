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
                    .Include(t => t.Perrito)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (adoptado == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<AdoptadoDTO>(adoptado));
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var adoptado = await context.Adoptados
                    .Include(t => t.Perrito)
                    .ToListAsync();

                if (!adoptado.Any())
                {
                    return NotFound();
                }

                return Ok(mapper.Map<List<AdoptadoDTO>>(adoptado));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor ", details = ex.Message });
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

    }
}