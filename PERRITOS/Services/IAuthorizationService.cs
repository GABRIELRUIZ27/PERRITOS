using Perritos.DTOs;

namespace Perritos.Services 
{ 
    public interface IAuthorizationService
    {
        Task<AppUserAuthDTO> ValidateUser(AppUserDTO dto);
    }
}