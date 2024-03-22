using AutoMapper;
using Perritos.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perritos;
using PERRITOS.DTOs;

namespace Perritos.Controllers
{
    [Route("api/tamaño")]
    [ApiController]
    public class TamañoController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
         
        public TamañoController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<TamañoDTO>>> GetAll()
        { 
            var tamaños = await context.Tamaños.ToListAsync();

            if (!tamaños.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<TamañoDTO>>(tamaños));
        }

    }
}