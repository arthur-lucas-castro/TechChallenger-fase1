using Microsoft.AspNetCore.Mvc;
using Cliente.Application.DTOs;
using Cliente.Application.Services.Interfaces;

namespace Cliente.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VeiculoController : ControllerBase
    {
        private readonly IVeiculoService _service;

        public VeiculoController(IVeiculoService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.ObterTodosAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var veiculo = await _service.ObterPorIdAsync(id);
            if (veiculo is null) return NotFound();
            return Ok(veiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VeiculoRequestDTO dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VeiculoRequestDTO dto)
        {
            if (!await _service.AtualizarAsync(id, dto)) return NotFound();
            return Ok(dto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.ExcluirAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
