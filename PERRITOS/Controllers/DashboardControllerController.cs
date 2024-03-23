
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perritos.DTOs;
using Perritos.Entities;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Perritos;
using PERRITOS.DTOs;

namespace simpatizantes_api.Controllers
{
    [Route("api/dashboard")]
    [ApiController]

    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public DashboardController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("total-perritos-por-edad")]
        public async Task<ActionResult<List<PerritoEdadDTO>>> TotalPerritosPorEdad() 
        {
            var Simpatizantes = await context.Perritos.ToListAsync();
            var totalSimpatizantes = Simpatizantes.Count;

            if (totalSimpatizantes == 0)
            {
                return Ok(new List<PerritoEdadDTO>());
            }

            var programasSociales = await context.Edades
                .Include(p => p.Perritos).ToListAsync();

            var estadisticas = programasSociales
                .Select(programa => new PerritoEdadDTO
                {
                    Id = programa.Id,
                    Nombre = programa.Nombre,
                    TotalPerritos = programa.Perritos?.Count ?? 0,
                    Porcentaje = programa.Perritos?.Count * 100 / totalSimpatizantes ?? 0
                })
                .ToList();

            return Ok(estadisticas);
        }

        [HttpGet("total-perritos-por-tamaño")]
        public async Task<ActionResult<List<PerritoTamañoDTO>>> TotalPerritosPorTamaño()
        {
            var Simpatizantes = await context.Perritos.ToListAsync();
            var totalSimpatizantes = Simpatizantes.Count;

            if (totalSimpatizantes == 0)
            {
                return Ok(new List<PerritoTamañoDTO>());
            }

            var programasSociales = await context.Tamaños 
                .Include(p => p.Perritos).ToListAsync();

            var estadisticas = programasSociales
                .Select(programa => new PerritoTamañoDTO
                {
                    Id = programa.Id,
                    Nombre = programa.Nombre,
                    TotalPerritos = programa.Perritos?.Count ?? 0,
                    Porcentaje = programa.Perritos?.Count * 100 / totalSimpatizantes ?? 0
                })
                .ToList();

            return Ok(estadisticas);
        }

        [HttpGet("total-perritos-por-genero")]
        public async Task<ActionResult<List<PerritoGeneroDTO>>> TotalPerritosPorGenero()
        {
            var Simpatizantes = await context.Perritos.ToListAsync();
            var totalSimpatizantes = Simpatizantes.Count;

            if (totalSimpatizantes == 0)
            {
                return Ok(new List<PerritoGeneroDTO>());
            }

            var programasSociales = await context.Generos
                .Include(p => p.Perritos).ToListAsync();

            var estadisticas = programasSociales
                .Select(programa => new PerritoGeneroDTO
                {
                    Id = programa.Id,
                    Nombre = programa.Nombre,
                    TotalPerritos = programa.Perritos?.Count ?? 0,
                    Porcentaje = programa.Perritos?.Count * 100 / totalSimpatizantes ?? 0
                })
                .ToList();

            return Ok(estadisticas);
        }
    }
}