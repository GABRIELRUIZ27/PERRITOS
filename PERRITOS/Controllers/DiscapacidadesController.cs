using AutoMapper;
using Perritos.DTOs;
using Perritos.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Perritos;

namespace Perritos.Controllers
{
    [Route("api/discapacidades")]
    [ApiController]
    public class DiscapacidadesController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper; 

        public DiscapacidadesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<DiscapacidadDTO>>> GetAll()
        {
            var discapacidades = await context.Discapacidades
                .OrderBy(u => u.Id)
                .ToListAsync();

            if (!discapacidades.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<DiscapacidadDTO>>(discapacidades));
        }

        [HttpPost("crear")]
        public async Task<ActionResult> Post(DiscapacidadDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var discapacidad = mapper.Map<Discapacidad>(dto);

                context.Add(discapacidad);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor al guardar el tipo de discapacidad.", details = ex.Message });
            }
        }

        [HttpDelete("eliminar/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var discapacidad = await context.Discapacidades.FindAsync(id);

            if (discapacidad == null)
            {
                return NotFound();
            }

            context.Discapacidades.Remove(discapacidad);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("actualizar/{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] DiscapacidadDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("El ID de la ruta y el ID del objeto no coinciden");
            }

            var discapacidad = await context.Discapacidades.FindAsync(id);

            if (discapacidad == null)
            {
                return NotFound();
            }

            mapper.Map(dto, discapacidad);

            context.Update(discapacidad);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscapacidadExists(id))
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

        private bool DiscapacidadExists(int id)
        {
            return context.Discapacidades.Any(e => e.Id == id);
        }

    }
}