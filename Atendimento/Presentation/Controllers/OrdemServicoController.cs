using Atendimento.Application.DTOs;
using Atendimento.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atendimento.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdemServicoController : ControllerBase
    {
        private readonly IOrdemServicoService _service;
        public OrdemServicoController(IOrdemServicoService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.ObterTodosAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var os = await _service.ObterPorIdAsync(id);
            if (os is null) return NotFound();
            return Ok(os);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrdemServicoRequestDTO dto)
        {
            var id = await _service.CriarAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrdemServicoRequestDTO dto)
        {
            if (!await _service.AtualizarAsync(id, dto)) return NotFound();
            return Ok(dto);
        }

        [HttpPatch("{id:int}/servicos/{servicoSolicitadoId:int}/execucao")]
        public async Task<IActionResult> AlterarStatusServicoExecucao(int id, int servicoSolicitadoId, [FromBody] AlterarStatusServicoExecucaoDTO dto)
        {
            try
            {
                if (!await _service.AlterarStatusServicoExecucaoAsync(id, servicoSolicitadoId, dto)) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> AlterarStatus(int id, [FromBody] AlterarStatusOrdemServicoDTO dto)
        {
            if (!await _service.AlterarStatusAsync(id, dto)) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:int}/servicos")]
        public async Task<IActionResult> AdicionarServico(int id, [FromBody] ServicoSolicitadoRequestDTO dto)
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
        public async Task<IActionResult> AdicionarPeca(int id, [FromBody] PecaSolicitadaRequestDTO dto)
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

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _service.ExcluirAsync(id)) return NotFound();
            return NoContent();
        }
    }
}
