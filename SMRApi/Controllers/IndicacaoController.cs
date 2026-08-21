using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Org.BouncyCastle.Bcpg.OpenPgp;
using SMRDominio.ClasseIndicacao;
using SMRDominio.ClassePessoa;
using SMRInfraestrutura;

namespace SMRApi.Controllers
{
    [ApiController]
    [Route("indicacao")]
    public class IndicacaoController : ControllerBase
    {
        private readonly SMRDBContext _dbContext;

        public IndicacaoController(SMRDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet("consultar-validacao/{codigoValidacao}")]
        [Authorize]
        public async Task<IActionResult> IndicacaoConsultarValidacao(string codigoValidacao)
        {
            try
            {
                if(string.IsNullOrEmpty(codigoValidacao))
                {
                    return BadRequest(new { Mensagem = "Dados inválidos pra consultar o codigo ùnico da indicação." });
                }
                var usuarioClaim = User.FindFirst("id_pessoa")?.Value
                      ?? User.FindFirst("id")?.Value
                      ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(usuarioClaim) || !int.TryParse(usuarioClaim, out int idPessoaLogada))
                {
                    return Unauthorized(new { Mensagem = "Usuário não autenticado." });
                }

                var empresa = await _dbContext.Empresa.Where(e => e.id_pessoa == idPessoaLogada).FirstOrDefaultAsync();

                var indicacaoConsultada = await (
                    from indicacao in _dbContext.Indicacao
                    join bonificacao in _dbContext.Bonificacoes on
                        indicacao.Id_Bonificacao equals bonificacao.Id
                    where (indicacao.Codigo_Validacao == codigoValidacao) &&
                          (bonificacao.Id_Empresa == empresa.id)
                    select new
                    {
                        IDIndicacao = indicacao.Id
                    }).FirstOrDefaultAsync();

                if(indicacaoConsultada == null)
                {
                    BadRequest(new { Mensagem = "Indicação não vinculada a empresa logada no sistema ou inexistente" });
                }

                return Ok(new
                {
                    IDIndicacao = indicacaoConsultada.IDIndicacao,
                    Mensagem = "Indicação encontrada com sucesso!"
                });
            }
            catch (Exception ex)
            {
                var detalhe = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Mensagem = "Erro ao consultar a indicação.", Erro = detalhe });
            }

        }

        [HttpPut("alterar-status/{codigoIndicacao}")]
        [Authorize]
        public async Task<IActionResult> IndicacaoAlterarStatus(int codigoIndicacao, [FromBody] IndicacaoStatus codigoStatus)
        {
            try
            {
                if (codigoIndicacao <= 0)
                {
                    return BadRequest(new { Mensagem = "Código de indicação inválido." });
                }

                // 1. Busca a indicação
                var indicacao = await _dbContext.Indicacao.FirstOrDefaultAsync(i => i.Id == codigoIndicacao);

                if (indicacao == null)
                {
                    return NotFound(new { Mensagem = $"Indicação com código {codigoIndicacao} não foi encontrada." });
                }

                // 2. Trata cada status
                if (codigoStatus == IndicacaoStatus.Enviada)
                {
                    // Gera um código de 8 dígitos se ainda não tiver um
                    if (string.IsNullOrEmpty(indicacao.Codigo_Validacao))
                    {
                        indicacao.Codigo_Validacao = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
                    }

                    indicacao.Status_Indicacao = IndicacaoStatus.Enviada;
                    await _dbContext.SaveChangesAsync();

                    string linkBase = $"{Request.Scheme}://{Request.Host}/indicacao/validar/{indicacao.Codigo_Validacao}";

                    return Ok(new
                    {


                        Mensagem = "Indicação enviada com sucesso!",
                        CodigoValidacao = indicacao.Codigo_Validacao,
                        LinkValidacao = linkBase
                    });
                }
                else if (codigoStatus == IndicacaoStatus.Cancelada)
                {
                    indicacao.Status_Indicacao = IndicacaoStatus.Cancelada;
                    await _dbContext.SaveChangesAsync();

                    return Ok(new { Mensagem = "Indicação cancelada com sucesso!" });
                }
                else
                {
                    indicacao.Status_Indicacao = codigoStatus;
                    await _dbContext.SaveChangesAsync();

                    return Ok(new { Mensagem = $"Status alterado para {codigoStatus} com sucesso!" });
                }
            }
            catch (Exception ex)
            {
                var detalhe = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Mensagem = "Erro ao alterar situação da indicação.", Erro = detalhe });
            }
        }


        [HttpGet("consultar-detalhes/{codigoIndicacao}")]
        [Authorize]
        public async Task<IActionResult> IndicacaoConsultarDetalhes(int codigoIndicacao)
        {
            try
            {
                if (codigoIndicacao <= 0)
                {
                    return BadRequest(new { Mensagem = "Dados inválidos para consultar os detalhes da indicação" });
                }

                var indicacaoDetalhe = await (
                     from indicacao in _dbContext.Indicacao.AsNoTracking()
                     where indicacao.Id == codigoIndicacao
                     join bonificacao in _dbContext.Bonificacoes.AsNoTracking() on indicacao.Id_Bonificacao equals bonificacao.Id
                     join promotor in _dbContext.Promotor.AsNoTracking() on indicacao.Id_Promotor_Indicou equals promotor.id
                     join empresa in _dbContext.Empresa.AsNoTracking() on bonificacao.Id_Empresa equals empresa.id
                     join pessoa in _dbContext.Pessoa.AsNoTracking() on promotor.id_pessoa equals pessoa.id_pessoa
                     select new IndicacaoDetalhesDto
                     {
                         IDIndicacao = indicacao.Id,
                         NomeIndicado = indicacao.Nome_Indicado,
                         CPF = indicacao.CPF,
                         TelefoneIndicado = indicacao.Telefone_Indicado,
                         StatusIndicacao = indicacao.Status_Indicacao.ToString(),
                         DataIndicacao = indicacao.Data_Indicacao,
                         DataValidacao = indicacao.Data_Validacao,
                         IDBonificacao = bonificacao.Id,
                         NomeBonificacao = bonificacao.Nome,
                         DescricaoBonificacao = bonificacao.Descricao,
                         ValorBonificacao = bonificacao.Valor,
                         IDEmpresa = empresa.id,
                         RazaoSocial = empresa.razao_social,
                         IDPromotor = promotor.id,
                         NomePromotor = pessoa.nome
                     }).FirstOrDefaultAsync();

                if (indicacaoDetalhe == null)
                {
                    return BadRequest(new { Mensagem = $"Indicação não encontrada para a consulta de detalhes, código consultado {codigoIndicacao}." });
                }

                return Ok(indicacaoDetalhe);
            }
            catch (Exception ex)
            {
                var detalhe = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Mensagem = "Erro ao consultar detalhes da indicação.", Erro = detalhe });
            }
        }

        [HttpPost ("cadastrar")]
        [Authorize]
        public async Task<IActionResult> IndicacaoCadastrar([FromBody] Indicacao indicacaoCadastro)
        {
            try
            {
               if(indicacaoCadastro == null)
                {
                    return BadRequest(new { Mensagem = "Dados inválidos para cadastro da indicação" });
                }

                var usuarioClaim = User.FindFirst("id_pessoa")?.Value
                        ?? User.FindFirst("id")?.Value
                        ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(usuarioClaim) || !int.TryParse(usuarioClaim, out int idPessoaLogada))
                {
                    return Unauthorized(new { Mensagem = "Usuário não autenticado." });
                }

                var pessoa = await _dbContext.Promotor.Where(p => p.id_pessoa == idPessoaLogada).FirstOrDefaultAsync();

                var indicacaoNova = new Indicacao
                {
                    Data_Indicacao = indicacaoCadastro.Data_Indicacao,
                    Data_Validacao = indicacaoCadastro.Data_Validacao,
                    Id_Promotor_Indicou = pessoa.id,
                    Nome_Indicado = indicacaoCadastro.Nome_Indicado,
                    Telefone_Indicado = indicacaoCadastro.Telefone_Indicado,
                    Status_Indicacao = indicacaoCadastro.Status_Indicacao,
                    Id_Bonificacao = indicacaoCadastro.Id_Bonificacao,
                    CPF = indicacaoCadastro.CPF
                };

                await _dbContext.Indicacao.AddAsync(indicacaoNova);
                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    Mensagem = $"Indicação cadastrada com sucesso para o indicado {indicacaoNova.Nome_Indicado}",
                    CodigoIndicacao = indicacaoNova.Id
                });
            }
            catch (Exception ex)
            {
                var detalhe = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Mensagem = "Erro ao cadastrar indicação.", Erro = detalhe });
            }
        }
    }
}
