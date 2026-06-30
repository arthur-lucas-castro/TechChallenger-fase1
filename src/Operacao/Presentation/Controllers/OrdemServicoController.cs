using Operacao.Application.DTOs;
using Operacao.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Operacao.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OrdemServicoController : ControllerBase
    {
        private readonly IOrdemServicoService _service;
        public OrdemServicoController(IOrdemServicoService service) => _service = service;

        [HttpGet]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> GetAll() => Ok(await _service.ObterTodosAsync());

        [HttpGet("{id:int}")]
        [ApiExplorerSettings(GroupName = "GestaoAdministrativa")]
        public async Task<IActionResult> GetById(int id)
        {
            var os = await _service.ObterDetalhadoPorIdAsync(id);
            if (os is null) return NotFound();
            return Ok(os);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrdemServicoRequestDto dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPost("{id:int}/servicos")]
        public async Task<IActionResult> AdicionarServico(int id, [FromBody] ServicoSolicitadoRequestDto dto)
        {
            try
            {
                if (!await _service.AdicionarServicoAsync(id, dto)) return NotFound();
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}/servicos/{servicoId:int}")]
        public async Task<IActionResult> RemoverServico(int id, int servicoId)
        {
            if (!await _service.RemoverServicoAsync(id, servicoId)) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:int}/pecas")]
        public async Task<IActionResult> AdicionarPeca(int id, [FromBody] PecaSolicitadaRequestDto dto)
        {
            try
            {
                if (!await _service.AdicionarPecaAsync(id, dto)) return NotFound();
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id:int}/pecas/{pecaId:int}")]
        public async Task<IActionResult> RemoverPeca(int id, int pecaId)
        {
            if (!await _service.RemoverPecaAsync(id, pecaId)) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:int}/confirmar-pagamento")]
        public async Task<IActionResult> ConfirmarPagamento(int id)
        {
            if (!await _service.ConfirmarPagamentoAsync(id)) return NotFound();
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
