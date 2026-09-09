using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMRApi.Services;
using SMRDominio.ClassePessoa;
using SMRDominio.ClasseRecompensa;
using SMRDominio.DTOs;
using SMRInfraestrutura;

namespace SMRApi.Controllers
{
    [ApiController]
    [Route ("recompensas")]
    public class RecompensaController : Controller
    {
        SMRDBContext _dbContext;

        public RecompensaController(SMRDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpPost("resgate")]
        [Authorize]
        public async Task<IActionResult> RecompensaResgate([FromBody] RecompensaResgate novoResgate)
        {
            try
            {
                if (novoResgate == null)
                {
                    return BadRequest(new { Mensagem = "Dados inválidos para o resgate de recompensas." });
                }

                var jaResgatada = await _dbContext.RecompensaResgates
                        .Where(rr => rr.id_recompensa == novoResgate.id_recompensa && rr.id_promotor == novoResgate.id_promotor).AnyAsync();

                if(jaResgatada)
                {
                    return BadRequest(new { Mensagem = "Essa recompensa já foi resgatada." });
                }

                var resgateNovo = new RecompensaResgate
                {
                    id_empresa = novoResgate.id_empresa,
                    id_promotor = novoResgate.id_promotor,
                    id_recompensa = novoResgate.id_recompensa,
                    data_resgate = novoResgate.data_resgate
                };

                _dbContext.RecompensaResgates.Add(resgateNovo);
                await _dbContext.SaveChangesAsync();

                return Ok(new {Mensagem = "Recompensa resgatada com sucesso!"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno ao consultar recompensas.", Erro = ex.Message });
            }

        }

        [HttpGet("consultar")]
        [Authorize]
        public async Task<IActionResult> ConsultarRecompensa([FromQuery] string? titulo, string? descricao, bool? ativo)
        {
            try
            {
                var pessoa = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id_pessoa")?.Value;
                int idPessoa = int.Parse(pessoa);
                var empresa = await _dbContext.Empresa.Where(e => e.id_pessoa == idPessoa).FirstOrDefaultAsync();

                int idEmpresa = empresa.id;

                var listaRecompensa = await (
                    from recompensa in _dbContext.Recompensas
                    where
                        (string.IsNullOrEmpty(titulo) || recompensa.titulo.Contains(titulo)) &&
                        (string.IsNullOrEmpty(descricao) || recompensa.descricao.Contains(descricao)) &&
                        (!ativo.HasValue || recompensa.Ativo == ativo) &&
                        (recompensa.id_empresa == idEmpresa)
                    select new
                    {
                        id = recompensa.id,
                        id_empresa = recompensa.id_empresa,
                        titulo = recompensa.titulo,
                        descricao = recompensa.descricao,
                        ativo = recompensa.Ativo,
                        pontos_necessarios = recompensa.pontos_necessarios,
                        id_rank = recompensa.id_rank
                    }).ToListAsync();


                return Ok(listaRecompensa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno ao consultar recompensas.", Erro = ex.Message });
            }
        }

        [HttpPost("cadastrar")]
        [Authorize]
        public async Task<IActionResult> CadastrarRecompensa([FromBody] Recompensa recompensa)
        {
            if (recompensa == null || !ModelState.IsValid)
            {
                return BadRequest(new { Message = "Dados da recompensa inválidos." });
            }

            try
            {
                var pessoa = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id_pessoa")?.Value;
                int idPessoa = int.Parse(pessoa);
                var empresa = await _dbContext.Empresa.Where(e => e.id_pessoa == idPessoa).FirstOrDefaultAsync();

                int idEmpresa = empresa.id;

                if (empresa == null)
                {
                    return BadRequest(new { Message = "Não foi possível cadastrar a recompensa porque nenhuma empresa está vinculada a este usuário." });
                }

                var novaRecompensa = new Recompensa
                {
                    id_empresa = idEmpresa,
                    titulo = recompensa.titulo,
                    descricao = recompensa.descricao,
                    pontos_necessarios = recompensa.pontos_necessarios,
                    Ativo = recompensa.Ativo,
                    id_rank = recompensa.id_rank
                };

                _dbContext.Recompensas.Add(novaRecompensa);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Message = "Recompensa cadastrada com sucesso!", Id = novaRecompensa.id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao cadastrar a recompensa", Detalhe = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("alterar")]
        [Authorize]
        public async Task<IActionResult> AlterarRecompensa([FromBody] Recompensa recompensa)
        {
            if (recompensa == null || !ModelState.IsValid)
            {
                return BadRequest(new { Message = "Dados da recompensa inválidos." });
            }

            try
            {
                var usuarioClaim = User.FindFirst("id_pessoa")?.Value;

                if (string.IsNullOrEmpty(usuarioClaim) || !int.TryParse(usuarioClaim, out int idPessoaLogada))
                {
                    return Unauthorized(new { Message = "Usuário não autenticado ou identificador inválido no token." });
                }
                var empresa = await _dbContext.Empresa
                    .FirstOrDefaultAsync(e => e.id_pessoa == idPessoaLogada);

                if (empresa == null)
                {
                    return BadRequest(new { Message = "Não foi possível alterar a recompensa porque nenhuma empresa está vinculada a este usuário." });
                }

                var recompesaAlterar = await _dbContext.Recompensas
                    .Where(a => a.id == recompensa.id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(u => u.titulo, recompensa.titulo)
                        .SetProperty(u => u.descricao, recompensa.descricao)
                        .SetProperty(u => u.Ativo, recompensa.Ativo)
                        .SetProperty(u => u.pontos_necessarios, recompensa.pontos_necessarios)
                        .SetProperty(u => u.id_rank,  recompensa.id_rank)
                    );
                
                

                if (recompesaAlterar <= 0)
                {
                    return NotFound(new { Mensagem = "Recompensa não encontrada para alteração." });
                }

                return Ok(new { Mensagem = "Recompensa alterada com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao cadastrar a recompensa", Detalhe = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("excluir{id}")]
        [Authorize]
        public async Task<IActionResult> ExcluirRecompensa(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Dados da recompensa inválidos." });
            }

            try
            {
                var usuarioClaim = User.FindFirst("id_pessoa")?.Value;

                if (string.IsNullOrEmpty(usuarioClaim) || !int.TryParse(usuarioClaim, out int idPessoaLogada))
                {
                    return Unauthorized(new { Message = "Usuário não autenticado ou identificador inválido no token." });
                }
                var empresa = await _dbContext.Empresa
                    .FirstOrDefaultAsync(e => e.id_pessoa == idPessoaLogada);

                if (empresa == null)
                {
                    return BadRequest(new { Message = "Não foi possível alterar a recompensa porque nenhuma empresa está vinculada a este usuário." });
                }

                //var recompensaUsada = await _dbContext.Recompensas
                //    .AnyAsync(r => r.id)

                var excluirRecompensa = await _dbContext.Recompensas
                    .Where(r => r.id == id && r.id_empresa == empresa.id).ExecuteDeleteAsync();

                if (excluirRecompensa <= 0)
                {
                    return NotFound(new { Mensagem = "Recompensa não encontrada para exclusão." });
                }

                return Ok(new { Mensagem = "Recompensa excluída com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Erro ao cadastrar a recompensa", Detalhe = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
