﻿using AutoMapper;
using Perritos.DTOs;
using Perritos;
using Perritos.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Perritos.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public AuthorizationService(ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            this.context = context;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public async Task<AppUserAuthDTO> ValidateUser(AppUserDTO dto)
        {
            var user = await (from u in context.Usuarios
                              join r in context.Rols
                              on u.Rol.Id equals r.Id
                              where u.Correo == dto.Email && u.Password == dto.Password
                              select new AppUserAuthDTO
                              {
                                  UsuarioId = u.Id,
                                  Email = u.Correo,
                                  RolId = r.Id,
                                  Rol = r.NombreRol
                              }).FirstOrDefaultAsync();

            if (user != null)
            {
                user.IsAuthenticated = true;
                user.Token = GenerateJwtToken(user.UsuarioId);
                user.Claims = await GeRolClaims(user.RolId);
            }

            return user;
        }


        private async Task<List<ClaimDTO>> GeRolClaims(int rolId)
        {
            var claims = await context.Claims.Where(c => c.Rol.Id == rolId).ToListAsync();
            return mapper.Map<List<ClaimDTO>>(claims);
        }

        public string GenerateJwtToken(int usuarioId)
        {
            var key = configuration.GetValue<string>("JwtSettings:key");

            // Asegúrate de que la longitud de la clave sea al menos 256 bits (32 bytes)
            if (Encoding.ASCII.GetBytes(key).Length < 32)
            {
                throw new InvalidOperationException("La longitud de la clave es menor de lo esperado.");
            }

            var keyBytes = Encoding.ASCII.GetBytes(key);

            // Describe las propiedades del usuario
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()));

            // Encripta la credencial de los tokens en a la clave en bytes 
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );

            // Describe el token en base a la propiedades, expiracion y la credencial
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = credentials
            };

            // Se cra un nuevo token a manipular instanciado de Jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            // Se escribe el nuevo token manipulado en base a las propiedades y su configuracion
            return tokenHandler.WriteToken(tokenConfig);
        }





    }
}