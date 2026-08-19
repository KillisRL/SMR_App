using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMRDominio.ClasseIndicacao;
using SMRDominio.DTOs;
using SMRInfraestrutura;
using System.Text;

namespace SMR_Api.Controllers
{
    [Route("indicacao/validar")]
    public class ValidacaoWebController : Controller
    {
        private readonly SMRDBContext _dbContext;

        public ValidacaoWebController(SMRDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{codigo}")]
        [AllowAnonymous]
        public async Task<IActionResult> ExibirValidacao(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return Content(GerarHtmlMensagem("Código Inválido", "O código de validação fornecido é inválido.", false), "text/html", Encoding.UTF8);
            }

            var dados = await (
                from indicacao in _dbContext.Indicacao.AsNoTracking()
                where indicacao.Codigo_Validacao == codigo
                join bonificacao in _dbContext.Bonificacoes.AsNoTracking() on indicacao.Id_Bonificacao equals bonificacao.Id
                join empresa in _dbContext.Empresa.AsNoTracking() on bonificacao.Id_Empresa equals empresa.id
                join promotor in _dbContext.Promotor.AsNoTracking() on indicacao.Id_Promotor_Indicou equals promotor.id
                join pessoa in _dbContext.Pessoa.AsNoTracking() on promotor.id_pessoa equals pessoa.id_pessoa
                select new
                {
                    indicacao.Id,
                    indicacao.Nome_Indicado,
                    indicacao.CPF,
                    indicacao.Status_Indicacao,
                    indicacao.Data_Indicacao,
                    indicacao.Data_Validacao,
                    indicacao.Codigo_Validacao,
                    NomeBonificacao = bonificacao.Nome,
                    DescricaoBonificacao = bonificacao.Descricao,
                    ValorBonificacao = bonificacao.Valor,
                    empresa.razao_social,
                    NomePromotor = pessoa.nome
                }).FirstOrDefaultAsync();

            if (dados == null)
            {
                return Content(GerarHtmlMensagem("Voucher Não Encontrado", "Nenhuma indicação encontrada para este código.", false), "text/html", Encoding.UTF8);
            }

            string html = GerarHtmlVoucher(
                dados.Codigo_Validacao,
                dados.Nome_Indicado,
                dados.CPF,
                dados.razao_social,
                dados.NomeBonificacao,
                dados.DescricaoBonificacao,
                dados.ValorBonificacao,
                dados.NomePromotor,
                dados.Status_Indicacao ?? IndicacaoStatus.Pendente,
                dados.Data_Validacao);

            return Content(html, "text/html", Encoding.UTF8);
        }

        [HttpPost("confirmar")]
        [Authorize]
        public async Task<IActionResult> ConfirmarValidacao([FromBody] ValidarCodigoDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.CodigoValidacao))
            {
                return BadRequest(new { Mensagem = "Código de validação não fornecido." });
            }

            var indicacao = await _dbContext.Indicacao.FirstOrDefaultAsync(i => i.Codigo_Validacao == dto.CodigoValidacao);

            if (indicacao == null)
            {
                return NotFound(new { Mensagem = "Indicação não encontrada para este código." });
            }

            if (indicacao.Status_Indicacao == IndicacaoStatus.Validada)
            {
                return BadRequest(new { Mensagem = "Esta indicação já foi validada anteriormente!" });
            }

            if (indicacao.Status_Indicacao == IndicacaoStatus.Cancelada)
            {
                return BadRequest(new { Mensagem = "Esta indicação foi cancelada e não pode ser validada." });
            }

            indicacao.Status_Indicacao = IndicacaoStatus.Validada;
            indicacao.Data_Validacao = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return Ok(new { Mensagem = "Indicação validada com sucesso! O bônus foi liberado." });
        }

        private string GerarHtmlVoucher(string codigo, string nomeIndicado, string cpf, string empresa, string bonificacao, string? descricao, decimal valor, string promotor, IndicacaoStatus status, DateTime? dataValidacao)
        {
            string statusBadge = status switch
            {
                IndicacaoStatus.Validada => "<span class='badge badge-success'>VALIDADA</span>",
                IndicacaoStatus.Enviada => "<span class='badge badge-warning'>AGUARDANDO VALIDAÇÃO</span>",
                IndicacaoStatus.Cancelada => "<span class='badge badge-danger'>CANCELADA</span>",
                _ => "<span class='badge badge-secondary'>PENDENTE</span>"
            };

            string rodapeInfo = status switch
            {
                IndicacaoStatus.Enviada => "<div class='aviso-instrucao'>Apresente este QR Code no caixa da empresa para validar seu benefício.</div>",
                IndicacaoStatus.Validada => $"<div class='aviso-sucesso'>✔ Validado em: {dataValidacao:dd/MM/yyyy HH:mm}</div>",
                _ => "<div class='aviso-invalido'>Este voucher não pode ser validado.</div>"
            };

            return $$"""
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Voucher de Indicação - SMR</title>
                <script src="https://cdnjs.cloudflare.com/ajax/libs/qrcodejs/1.0.0/qrcode.min.js"></script>
                <style>
                    * { box-sizing: border-box; margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; }
                    body { background-color: #121212; color: #E0E0E0; display: flex; justify-content: center; align-items: center; min-height: 100vh; padding: 15px; }
                    .card { background-color: #1E1E1E; border-radius: 16px; border: 1px solid #2D2D2D; width: 100%; max-width: 420px; padding: 24px; box-shadow: 0 10px 30px rgba(0,0,0,0.5); text-align: center; }
                    .header h1 { font-size: 18px; color: #D4AF37; letter-spacing: 1px; margin-bottom: 4px; }
                    .header p { font-size: 12px; color: #888; margin-bottom: 20px; }
                    .qrcode-container { background: #FFFFFF; padding: 16px; border-radius: 12px; display: inline-block; margin-bottom: 16px; }
                    .codigo-box { background: #252525; padding: 10px; border-radius: 8px; font-family: monospace; font-size: 20px; font-weight: bold; letter-spacing: 3px; color: #D4AF37; margin-bottom: 20px; border: 1px dashed #D4AF37; }
                    .info-group { text-align: left; background: #252525; padding: 14px; border-radius: 10px; margin-bottom: 16px; font-size: 13px; line-height: 1.6; }
                    .info-group strong { color: #FFF; }
                    .info-label { color: #888; font-size: 11px; text-transform: uppercase; }
                    .badge { padding: 4px 10px; border-radius: 20px; font-size: 11px; font-weight: bold; }
                    .badge-warning { background: rgba(212, 175, 55, 0.2); color: #D4AF37; border: 1px solid #D4AF37; }
                    .badge-success { background: rgba(46, 204, 113, 0.2); color: #2ecc71; border: 1px solid #2ecc71; }
                    .badge-danger { background: rgba(231, 76, 60, 0.2); color: #e74c3c; border: 1px solid #e74c3c; }
                    .badge-secondary { background: rgba(150, 150, 150, 0.2); color: #aaa; border: 1px solid #aaa; }
                    .aviso-instrucao { color: #D4AF37; font-size: 13px; font-weight: 500; border: 1px dashed rgba(212, 175, 55, 0.5); padding: 12px; border-radius: 8px; background: rgba(212, 175, 55, 0.05); }
                    .aviso-sucesso { color: #2ecc71; font-weight: bold; padding: 10px; font-size: 13px; }
                    .aviso-invalido { color: #e74c3c; font-weight: bold; padding: 10px; font-size: 13px; }
                </style>
            </head>
            <body>
                <div class="card">
                    <div class="header">
                        <h1>SMR - MARKETING DE INDICAÇÃO</h1>
                        <p>Voucher de Validação Presencial</p>
                    </div>

                    <div class="qrcode-container" id="qrcode"></div>

                    <div class="codigo-box">{{codigo}}</div>

                    <div style="margin-bottom: 15px;">{{statusBadge}}</div>

                    <div class="info-group">
                        <div class="info-label">Empresa Parceira</div>
                        <div><strong>{{empresa}}</strong></div>
                        
                        <div class="info-label" style="margin-top:8px;">Benefício / Bônus</div>
                        <div><strong>{{bonificacao}}</strong> (R$ {{valor:N2}})</div>

                        <div class="info-label" style="margin-top:8px;">Cliente Indicado</div>
                        <div><strong>{{nomeIndicado}}</strong> - CPF: {{cpf}}</div>

                        <div class="info-label" style="margin-top:8px;">Indicado por</div>
                        <div>{{promotor}}</div>
                    </div>

                    {{rodapeInfo}}
                </div>

                <script>
                    new QRCode(document.getElementById("qrcode"), {
                        text: "{{codigo}}",
                        width: 170,
                        height: 170,
                        colorDark : "#000000",
                        colorLight : "#ffffff",
                        correctLevel : QRCode.CorrectLevel.H
                    });
                </script>
            </body>
            </html>
            """;
        }

        private string GerarHtmlMensagem(string titulo, string mensagem, bool sucesso)
        {
            string cor = sucesso ? "#2ecc71" : "#e74c3c";
            return $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>{{titulo}}</title>
                <style>
                    body { background: #121212; color: #fff; font-family: sans-serif; display: flex; justify-content: center; align-items: center; height: 100vh; margin:0; text-align: center; }
                    .box { background: #1E1E1E; padding: 30px; border-radius: 12px; border: 1px solid #333; max-width: 350px; }
                    h2 { color: {{cor}}; margin-bottom: 10px; }
                    p { color: #AAA; }
                </style>
            </head>
            <body>
                <div class="box">
                    <h2>{{titulo}}</h2>
                    <p>{{mensagem}}</p>
                </div>
            </body>
            </html>
            """;
        }
    }
}