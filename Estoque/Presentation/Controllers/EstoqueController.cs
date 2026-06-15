using Microsoft.AspNetCore.Mvc;
using Estoque.Application.DTOs;
using Estoque.Application.Services.Interfaces;

namespace Estoque.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly IPecaService _service;

        public EstoqueController(IPecaService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.ObterEstoqueTodosAsync());

        [HttpPost("entrada")]
        public async Task<IActionResult> AdicionarProduto([FromBody] EntradaEstoqueRequestDTO dto)
        {
            try
            {
                var resultado = await _service.AdicionarEstoqueAsync(dto);
                return Ok(resultado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("baixa")]
        public async Task<IActionResult> DarBaixa([FromBody] BaixaEstoqueRequestDTO dto)
        {
            try
            {
                var resultado = await _service.DarBaixaAsync(dto);
                if (resultado is null)
                    return NotFound($"Estoque não encontrado para a peça {dto.PecaId}.");
                return Ok(resultado);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
