using Microsoft.AspNetCore.Mvc;
using Estoque.Application.DTOs;
using Estoque.Application.Services.Interfaces;

namespace Estoque.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly IEstoqueService _service;

        public EstoqueController(IEstoqueService service) => _service = service;

        [HttpPost("entrada")]
        public async Task<IActionResult> AdicionarProduto([FromBody] EntradaEstoqueRequestDTO dto)
        {
            var resultado = await _service.AdicionarProdutoAsync(dto);
            return Ok(resultado);
        }
    }
}
