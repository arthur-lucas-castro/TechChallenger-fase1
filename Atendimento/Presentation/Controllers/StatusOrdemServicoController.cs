using Atendimento.Application.DTOs;
using Atendimento.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atendimento.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StatusOrdemServicoController : ControllerBase
    {
        private readonly IOrdemServicoService _service;
        public StatusOrdemServicoController(IOrdemServicoService service) => _service = service;

        [HttpPatch("{id:int}/iniciar-diagnostico")]
        public async Task<IActionResult> IniciarDiagnostico(int id)
        {
            if (!await _service.IniciarDiagnosticoAsync(id)) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}/finalizar-diagnostico")]
        public async Task<IActionResult> FinalizarDiagnostico(int id)
        {
            if (!await _service.FinalizarDiagnosticoAsync(id)) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> AlterarStatus(int id, [FromBody] AlterarStatusOrdemServicoDTO dto)
        {
            if (!await _service.AlterarStatusAsync(id, dto)) return NotFound();
            return NoContent();
        }

        [HttpPatch("{ordemServicoId:int}/iniciar-servico/{servicoSolicitadoId:int}")]
        public async Task<IActionResult> IniciarServico(int ordemServicoId, int servicoSolicitadoId)
        {
            if (!await _service.IniciarServicoAsync(ordemServicoId, servicoSolicitadoId)) return NotFound();
            return NoContent();
        }

        [HttpPatch("{ordemServicoId:int}/finalizar-servico/{servicoSolicitadoId:int}")]
        public async Task<IActionResult> FinalizarServico(int ordemServicoId, int servicoSolicitadoId)
        {
            if (!await _service.FinalizarServicoAsync(ordemServicoId, servicoSolicitadoId)) return NotFound();
            return NoContent();
        }

        [HttpGet("tempos-servico")]
        public async Task<IActionResult> GetTemposServico()
            => Ok(await _service.ObterTemposExecucaoPorServicoAsync());
    }
}
