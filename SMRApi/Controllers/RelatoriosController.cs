using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMRApi.Repositories;
using SMRInfraestrutura;

namespace SMRApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Exige que o usuário esteja logado com o token JWT
    public class RelatoriosController : ControllerBase
    {
        private readonly RelatoriosRepository _repository;
        private readonly SMRDBContext _dbContext;

        public RelatoriosController(RelatoriosRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("custo-indicacao")]
        [Authorize]
        public async Task<IActionResult> GetCustoIndicacao(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                if (inicio > fim)
                    return BadRequest(new { message = "A data de início não pode ser maior que a data de fim." });

                var pessoa = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id_pessoa")?.Value;

                if (pessoa == null) 
                    return BadRequest(new { message = "Usuário não autenticado." });

                int idPessoa = int.Parse(pessoa);

                var empresa = await _dbContext.Empresa.Where(e => e.id_pessoa == idPessoa).FirstOrDefaultAsync();

                if (empresa == null) 
                    return BadRequest(new { message = "Empresa não encontrada para o usuário logado." }); 
                int idEmpresa = empresa.id;

                var dados = await _repository.ObterCustoIndicacaoAsync(inicio, fim, idEmpresa);
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }
    }
}