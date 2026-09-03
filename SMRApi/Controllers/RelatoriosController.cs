using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using SMRApi.Repositories;
using SMRInfraestrutura;

namespace SMRApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize] // Exige que o usuário esteja logado com o token JWT
    public class RelatoriosController : ControllerBase
    {
        private readonly RelatoriosRepository _repository;
        private readonly SMRDBContext _dbContext;

        public RelatoriosController(RelatoriosRepository repository, SMRDBContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        private async Task<int> ObterIdEmpresaLogadaAsync()
        {
            var pessoa = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("id_pessoa")?.Value;

            if (pessoa == null)
                throw new Exception("Usuário não autenticado.");

            int idPessoa = int.Parse(pessoa);

            var empresa = await _dbContext.Empresa.Where(e => e.id_pessoa == idPessoa).FirstOrDefaultAsync();

            if (empresa == null)
                throw new Exception("Empresa não encontrada para o usuário logado.");

            return empresa.id;
        }

        [HttpGet("custo-indicacao")]
        [Authorize]
        public async Task<IActionResult> GetCustoIndicacao([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            try
            {
                if (inicio > fim)
                    return BadRequest(new { message = "A data de início não pode ser maior que a data de fim." });

                int idEmpresa = await ObterIdEmpresaLogadaAsync(); // Linha limpa e reutilizável!

                var dados = await _repository.ObterCustoIndicacaoAsync(inicio, fim, idEmpresa);
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        [HttpGet("exportar-excel")]
        [Authorize]
        public async Task<IActionResult> ExportarExcel([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            try
            {
                int idEmpresa = await ObterIdEmpresaLogadaAsync(); // Reutilizando aqui também!
                var dados = await _repository.ObterDetalhesExportacaoAsync(inicio, fim, idEmpresa);

                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Custo Bonificação");

                worksheet.Cell("A1").Value = "Data";
                worksheet.Cell("B1").Value = "Descrição";
                worksheet.Cell("C1").Value = "Valor (R$)";

                int linha = 2;
                foreach (var item in dados)
                {
                    worksheet.Cell(linha, 1).Value = item.DataIndicacao.ToString("dd/MM/yyyy");
                    worksheet.Cell(linha, 2).Value = item.DescricaoBonificacao;
                    worksheet.Cell(linha, 3).Value = item.Valor;
                    linha++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Relatorio_Bonificacao_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao gerar Excel: {ex.Message}" });
            }
        }

        [HttpGet("exportar-pdf")]
        [Authorize]
        public async Task<IActionResult> ExportarPdf([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

                int idEmpresa = await ObterIdEmpresaLogadaAsync(); // E reutilizando aqui também!
                var dados = await _repository.ObterDetalhesExportacaoAsync(inicio, fim, idEmpresa);

                var documento = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Header().Text("Relatório de Controle de Bonificação").FontSize(20).Bold().FontColor("#1E3A8A");

                        page.Content().Column(col =>
                        {
                            col.Spacing(10);
                            col.Item().Text($"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}").FontSize(12);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(100);
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(80);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background("#E2E8F0").Padding(5).Text("Data").Bold();
                                    header.Cell().Background("#E2E8F0").Padding(5).Text("Descrição").Bold();
                                    header.Cell().Background("#E2E8F0").Padding(5).Text("Valor").Bold();
                                });

                                foreach (var item in dados)
                                {
                                    table.Cell().BorderBottom(1).BorderColor("#CBD5E1").Padding(5).Text(item.DataIndicacao.ToString("dd/MM/yyyy"));
                                    table.Cell().BorderBottom(1).BorderColor("#CBD5E1").Padding(5).Text(item.DescricaoBonificacao ?? "");
                                    table.Cell().BorderBottom(1).BorderColor("#CBD5E1").Padding(5).Text($"R$ {item.Valor:N2}");
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                    });
                });

                var pdfBytes = documento.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"Relatorio_Bonificacao_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao gerar PDF: {ex.Message}" });
            }
        }
    }
}