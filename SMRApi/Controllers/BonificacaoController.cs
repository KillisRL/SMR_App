using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMRDominio.ClasseBonificacao;
using SMRDominio.ClassePessoa;
using SMRInfraestrutura;

namespace SMRApi.Controllers
{
    [ApiController]
    [Route("bonificacao")]
    public class BonificacaoController : ControllerBase
    {
        private SMRDBContext _dbContext;

        public BonificacaoController(SMRDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpDelete("excluir{id}")]
        [Authorize]
        public async Task<IActionResult> ExcluirBonificacao(int codigoBonificacao)
        {
            try
            {
                var pessoa = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id_pessoa")?.Value;
                int idPessoa = int.Parse(pessoa);

                var empresa = await _dbContext.Empresa.Where(e => e.id_pessoa == idPessoa).FirstOrDefaultAsync();

                int idEmpresa = empresa.id;

                var bonificacaoExcluida = await _dbContext.Bonificacoes
                    .Where(b => b.Id == codigoBonificacao && b.Id_Empresa == idEmpresa).ExecuteDeleteAsync();

                if(bonificacaoExcluida <= 0 )
                {
                    return BadRequest(new { Mensagem = "Bonificação não encontrada para exclusão!" });
                }

                return Ok(new { Mensagem = "Bonificação excluída com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = $"Erro ao alterar bonificação: {ex.Message}" });
            }
        }

        [HttpPut("alterar")]
        [Authorize]
        public async Task<IActionResult> AlterarBonificacao([FromBody] Bonificacao bonificacao)
        {
            try
            {
                var pessoa = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id_pessoa")?.Value;
                int idPessoa = int.Parse(pessoa);

                var empresa = await _dbContext.Empresa.Where(e => e.id_pessoa == idPessoa).FirstOrDefaultAsync();

                int idEmpresa = empresa.id;

                if (bonificacao == null || !ModelState.IsValid)
                {
                    return BadRequest(new { Mensagem = "Dados inválidos para alteração" });
                }


                var bonificacaoAlterada = await _dbContext.Bonificacoes
                    .Where(b => b.Id == bonificacao.Id && b.Id_Empresa == idEmpresa)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(b => b.Descricao, bonificacao.Descricao)
                        .SetProperty(b => b.Nome, bonificacao.Nome)
                        .SetProperty(b => b.Ativo, bonificacao.Ativo)
                        .SetProperty(b => b.Tipo, bonificacao.Tipo)
                        .SetProperty(b => b.Mgm, bonificacao.Mgm)
                        .SetProperty(b => b.Valor, bonificacao.Valor));

                if(bonificacaoAlterada <= 0)
                {
                    return BadRequest(new { Mensagem = "Bonificação não encontrada para alteração" });
                }
                return Ok(new { Mensagem = "Bonificação alterada com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = $"Erro ao alterar bonificação: {ex.Message}" });
            }
        }

        [HttpGet("consultar")]
        [Authorize]
        public async Task<IActionResult> ConsultarBonificacao([FromQuery] string? nome, [FromQuery] bool? ativo)
        {
            try
            {

                var usuarioClaim = User.FindFirst("id_pessoa")?.Value
                                ?? User.FindFirst("id")?.Value
                                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(usuarioClaim) || !int.TryParse(usuarioClaim, out int idPessoaLogada))
                {
                    return Unauthorized(new { Mensagem = "Usuário não autenticado." });
                }

                var empresa = await _dbContext.Empresa.FirstOrDefaultAsync(e => e.id_pessoa == idPessoaLogada);

                int idEmpresaFinal = empresa != null ? empresa.id : idPessoaLogada;
                var query = _dbContext.Bonificacoes.AsNoTracking().Where(b => b.Id_Empresa == idEmpresaFinal);

                if (!string.IsNullOrWhiteSpace(nome))
                {
                    query = query.Where(b => b.Nome != null && b.Nome.Contains(nome));
                }

                if (ativo.HasValue)
                {
                    query = query.Where(b => b.Ativo == ativo.Value);
                }
                var listaBonificacao = await query.Select(bonificacao => new
                {
                    Id = bonificacao.Id,
                    Id_empresa = bonificacao.Id_Empresa,
                    Nome = bonificacao.Nome,
                    Descricao = bonificacao.Descricao,
                    Valor = bonificacao.Valor,
                    Tipo = bonificacao.Tipo,
                    Mgm = bonificacao.Mgm,   
                    Ativo = bonificacao.Ativo 
                }).ToListAsync();

                return Ok(listaBonificacao);
            }
            catch (Exception ex)
            {
                var detalhe = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Mensagem = "Erro ao consultar bonificação.", Erro = detalhe });
            }
        }

        [HttpPost("cadastrar")]
        [Authorize]
        public async Task<IActionResult> CadastrarBonificacao([FromBody] Bonificacao bonificacao)
        {
            try
            {
                if (bonificacao == null || string.IsNullOrWhiteSpace(bonificacao.Nome) || string.IsNullOrWhiteSpace(bonificacao.Tipo.ToString()))
                {
                    return BadRequest(new { Mensagem = "Os campos Nome e Tipo são obrigatórios." });
                }
                var usuarioClaim = User.FindFirst("id_pessoa")?.Value
                                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(usuarioClaim) || !int.TryParse(usuarioClaim, out int idPessoaLogada))
                {
                    return Unauthorized(new { Mensagem = "Usuário não autenticado." });
                }

                var empresa = await _dbContext.Empresa.FirstOrDefaultAsync(e => e.id_pessoa == idPessoaLogada);

                int idEmpresaFinal = empresa != null ? empresa.id : idPessoaLogada;
                var novaBonificacao = new Bonificacao
                {
                    Id_Empresa = idEmpresaFinal,
                    Nome = bonificacao.Nome,
                    Descricao = bonificacao.Descricao ?? string.Empty,
                    Valor = bonificacao.Valor,
                    Tipo = bonificacao.Tipo,
                    Mgm= bonificacao.Mgm,
                    Ativo = bonificacao.Ativo
                };

                await _dbContext.Bonificacoes.AddAsync(novaBonificacao);
                await _dbContext.SaveChangesAsync();

                return Ok(new { Mensagem = "Bonificação cadastrada com sucesso!" });
            }
            catch (Exception ex)
            {
                var detalhe = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Mensagem = "Erro ao cadastrar bonificação.", Erro = detalhe });
            }
        }
    }
}
