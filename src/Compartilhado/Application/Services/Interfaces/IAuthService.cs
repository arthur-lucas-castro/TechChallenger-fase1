using Compartilhado.Application.DTOs;

namespace Compartilhado.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO dto);
    }
}
