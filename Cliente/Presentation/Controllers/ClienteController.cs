using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Cliente.Application.DTOs;
using Cliente.Application.Services.Interfaces;

namespace Cliente.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClienteController(IClienteService service) => _service = service;

        [HttpGet]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> GetAll() => Ok(await _service.ObterTodosAsync());

        [HttpGet("{id:int}")]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _service.ObterPorIdAsync(id);
            if (cliente is null) return NotFound();
            return Ok(cliente);
        }

        [HttpGet("{documentNumber}")]
        public async Task<IActionResult> GetByDocument(string documentNumber)
        {
            var cliente = await _service.ObterPorNumeroDocumentoAsync(documentNumber);
            if (cliente is null) return NotFound();
            return Ok(cliente);
        }

        [HttpPost]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> Create([FromBody] ClienteRequestDTO dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> Update(int id, [FromBody] ClienteRequestDTO dto)
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

        [HttpPost("{clienteId:int}/ordens-servico/{ordemServicoId:int}/responder-orcamento")]
        public async Task<IActionResult> ResponderOrcamento(int clienteId, int ordemServicoId, [FromBody] ResponderOrcamentoDTO dto)
        {
            if (!await _service.ResponderOrcamentoAsync(clienteId, ordemServicoId, dto)) return NotFound();
            return NoContent();
        }
    }
}
