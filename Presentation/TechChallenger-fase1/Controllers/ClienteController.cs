using Application.Servicos.DTOs;
using Application.Servicos.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.TechChallenger_fase1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClienteController(IClienteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clientes = await _service.ObterTodosAsync();
            return Ok(clientes);
        }

        [HttpGet("{documentNumber}")]

        public async Task<IActionResult> GetByDocumentNumber(string documentNumber)
        {
            var clientes = await _service.ObterPorNumeroDocumentoAsync(documentNumber);
            return Ok(clientes);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _service.ObterPorIdAsync(id);
            if (cliente is null) return NotFound();
            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteRequestDTO dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteRequestDTO dto)
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
