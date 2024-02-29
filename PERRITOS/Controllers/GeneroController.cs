using AutoMapper;
using Perritos.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perritos;

namespace Perritos.Controllers 
{
    [Route("api/genero")]
    [ApiController]
    public class GeneroController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GeneroController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<GeneroDTO>>> GetAll()
        {
            var generos = await context.Generos.ToListAsync();

            if (!generos.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<GeneroDTO>>(generos));
        }

    }
}