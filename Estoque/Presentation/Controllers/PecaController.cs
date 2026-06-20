using Microsoft.AspNetCore.Mvc;
using Estoque.Application.DTOs;
using Estoque.Application.Services.Interfaces;

namespace Estoque.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PecaController : ControllerBase
    {
        private readonly IPecaService _service;

        public PecaController(IPecaService service) => _service = service;

        [HttpGet]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.ObterTodosAsync());

        [HttpGet("{id:int}")]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> GetById(int id)
        {
            var peca = await _service.ObterPorIdAsync(id);
            if (peca is null) return NotFound();
            return Ok(peca);
        }

        [HttpPost]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> Create([FromBody] PecaRequestDTO dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> Update(int id, [FromBody] PecaRequestDTO dto)
        {
            if (!await _service.AtualizarAsync(id, dto)) return NotFound();
            return Ok(dto);
        }

        [HttpDelete("{id:int}")]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.ExcluirAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
