using Application.Servicos.DTOs;
using Application.Servicos.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.TechChallenger_fase1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemServicoController : ControllerBase
    {
        private readonly IItemServicoService _service;

        public ItemServicoController(IItemServicoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var itens = await _service.ObterTodosAsync();
            return Ok(itens);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.ObterPorIdAsync(id);
            if (item is null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ItemServicoRequestDTO dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemServicoRequestDTO dto)
        {
            if (!await _service.AtualizarAsync(id, dto)) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.ExcluirAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
