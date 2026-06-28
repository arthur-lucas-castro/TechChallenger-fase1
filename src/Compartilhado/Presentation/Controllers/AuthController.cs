using Compartilhado.Application.DTOs;
using Compartilhado.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Compartilhado.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service) => _service = service;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var resultado = await _service.LoginAsync(dto);
            if (resultado is null) return Unauthorized();
            return Ok(resultado);
        }
    }
}
