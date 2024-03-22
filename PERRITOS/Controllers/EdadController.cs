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
    [Route("api/edad")]
    [ApiController]
    public class EdadController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public EdadController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("obtener-todos")]
        public async Task<ActionResult<List<EdadDTO>>> GetAll()
        {
            var edades = await context.Edades.ToListAsync();

            if (!edades.Any())
            {
                return NotFound();
            }

            return Ok(mapper.Map<List<EdadDTO>>(edades));
        }

    }
}