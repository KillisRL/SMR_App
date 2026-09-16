using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using SMRApi.Repositories;
using SMRDominio.DTOs;
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

                int idEmpresa = await ObterIdEmpresaLogadaAsync();

                var dados = await _repository.ObterCustoIndicacaoAsync(inicio, fim, idEmpresa);
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        [HttpGet("ranking-promotores")]
        [Authorize]
        public async Task<IActionResult> GetRankingPromotores(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim,
            [FromQuery] int status = 0)
        {
            try
            {
                if (inicio > fim)
                    return BadRequest(new { message = "A data de início não pode ser maior que a data de fim." });

                int idEmpresa = await ObterIdEmpresaLogadaAsync();

                var dados = await _repository.ObterRankingPromotoresAsync(inicio, fim, idEmpresa, status);

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

        // =========================================================================
        // EXPORTAÇÕES DO RANKING DE PROMOTORES
        // =========================================================================

        [HttpGet("ranking-promotores/excel")]
        [Authorize]
        public async Task<IActionResult> ExportarRankingExcel([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim, [FromQuery] int status = 0)
        {
            try
            {
                int idEmpresa = await ObterIdEmpresaLogadaAsync();
                var dados = await _repository.ObterRankingPromotoresAsync(dataInicio, dataFim, idEmpresa, status);

                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Ranking Promotores");

                worksheet.Cell("A1").Value = "Nome do Promotor";
                worksheet.Cell("B1").Value = "Total de Indicações";

                // Estilo do cabeçalho
                var headerRange = worksheet.Range("A1:B1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;

                int linha = 2;
                foreach (var item in dados.OrderByDescending(d => d.Quantidade))
                {
                    worksheet.Cell(linha, 1).Value = item.NomePromotor;
                    worksheet.Cell(linha, 2).Value = item.Quantidade;
                    linha++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Ranking_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao gerar Excel do Ranking: {ex.Message}" });
            }
        }

        [HttpGet("ranking-promotores/pdf")]
        [Authorize]
        public async Task<IActionResult> ExportarRankingPdf([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim, [FromQuery] int status = 0)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                int idEmpresa = await ObterIdEmpresaLogadaAsync();
                var dados = await _repository.ObterRankingPromotoresAsync(dataInicio, dataFim, idEmpresa, status);

                var documento = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Header().Text("Ranking de Promotores").FontSize(20).Bold().FontColor("#D4AF37"); // Dourado

                        page.Content().Column(col =>
                        {
                            col.Spacing(10);
                            col.Item().Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}").FontSize(12);
                            col.Item().Text($"Status Filtrado: {(status == 0 ? "Todos" : status.ToString())}").FontSize(12);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(120);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background("#333333").Padding(5).Text("Nome do Promotor").FontColor("#FFFFFF").Bold();
                                    header.Cell().Background("#333333").Padding(5).Text("Total de Indicações").FontColor("#FFFFFF").Bold().AlignCenter();
                                });

                                foreach (var item in dados.OrderByDescending(d => d.Quantidade))
                                {
                                    table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(5).Text(item.NomePromotor ?? "");
                                    table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(5).Text(item.Quantidade.ToString()).AlignCenter();
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(x => { x.Span("Página "); x.CurrentPageNumber(); });
                    });
                });

                return File(documento.GeneratePdf(), "application/pdf", $"Ranking_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao gerar PDF do Ranking: {ex.Message}" });
            }
        }

        // =========================================================================
        // MÉTRICAS DE CONVERSÃO (O NOVO RELATÓRIO)
        // =========================================================================

        [HttpGet("conversao")]
        [Authorize]
        public async Task<IActionResult> GetMetricasConversao([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                if (dataInicio > dataFim)
                    return BadRequest(new { message = "A data de início não pode ser maior que a data de fim." });

                // int idEmpresa = await ObterIdEmpresaLogadaAsync();

                // 1. Busca todas as indicações do período
                // CORREÇÃO: Usando Indicacao (singular) e Data_Indicacao (com D e I maiúsculos)
                var indicacoes = await _dbContext.Indicacao
                    .Where(i => i.Data_Indicacao >= dataInicio && i.Data_Indicacao <= dataFim)
                    // TODO: Para filtrar por empresa depois, faça um join com Bonificacao:
                    // .Where(i => _dbContext.Bonificacoes.Any(b => b.Id == i.Id_Bonificacao && b.id_empresa == idEmpresa))
                    .ToListAsync();

                // 2. Agrupa por mês e ano e calcula a conversão
                var metricas = indicacoes
                    .GroupBy(i => new { i.Data_Indicacao.Year, i.Data_Indicacao.Month })
                    .Select(g => new
                    {
                        Mes = $"{g.Key.Month:D2}/{g.Key.Year}",
                        TotalEnviadas = g.Count(),
                        // CORREÇÃO: Usando Status_Indicacao
                        TotalValidadas = g.Count(i => i.Status_Indicacao == SMRDominio.ClasseIndicacao.IndicacaoStatus.Validada),
                        Ano = g.Key.Year,
                        MesNum = g.Key.Month
                    })
                    .OrderBy(m => m.Ano).ThenBy(m => m.MesNum)
                    .Select(m => new MetricaConversaoDTO
                    {
                        Mes = m.Mes,
                        TaxaConversao = m.TotalEnviadas > 0
                            ? Math.Round(((double)m.TotalValidadas / m.TotalEnviadas) * 100, 2)
                            : 0
                    })
                    .ToList();

                return Ok(metricas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        [HttpGet("conversao/excel")]
        [Authorize]
        public async Task<IActionResult> ExportarConversaoExcel([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                // int idEmpresa = await ObterIdEmpresaLogadaAsync();

                // CORREÇÃO APLICADA AQUI TAMBÉM
                var indicacoes = await _dbContext.Indicacao
                    .Where(i => i.Data_Indicacao >= dataInicio && i.Data_Indicacao <= dataFim)
                    .ToListAsync();

                var dados = indicacoes.GroupBy(i => new { i.Data_Indicacao.Year, i.Data_Indicacao.Month })
                    .Select(g => new {
                        Mes = $"{g.Key.Month:D2}/{g.Key.Year}",
                        TotalEnviadas = g.Count(),
                        TotalValidadas = g.Count(i => i.Status_Indicacao == SMRDominio.ClasseIndicacao.IndicacaoStatus.Validada),
                        Ano = g.Key.Year,
                        MesNum = g.Key.Month
                    }).OrderBy(m => m.Ano).ThenBy(m => m.MesNum).ToList();

                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Métricas de Conversão");

                worksheet.Cell("A1").Value = "Mês/Ano";
                worksheet.Cell("B1").Value = "Indicações Enviadas";
                worksheet.Cell("C1").Value = "Indicações Validadas";
                worksheet.Cell("D1").Value = "Taxa de Conversão (%)";

                var headerRange = worksheet.Range("A1:D1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;

                int linha = 2;
                foreach (var item in dados)
                {
                    double taxa = item.TotalEnviadas > 0 ? ((double)item.TotalValidadas / item.TotalEnviadas) * 100 : 0;

                    worksheet.Cell(linha, 1).Value = item.Mes;
                    worksheet.Cell(linha, 2).Value = item.TotalEnviadas;
                    worksheet.Cell(linha, 3).Value = item.TotalValidadas;
                    worksheet.Cell(linha, 4).Value = Math.Round(taxa, 2);
                    linha++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Conversao_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao gerar Excel de Conversão: {ex.Message}" });
            }
        }

        [HttpGet("conversao/pdf")]
        [Authorize]
        public async Task<IActionResult> ExportarConversaoPdf([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
                // int idEmpresa = await ObterIdEmpresaLogadaAsync();

                // CORREÇÃO APLICADA AQUI TAMBÉM
                var indicacoes = await _dbContext.Indicacao
                    .Where(i => i.Data_Indicacao >= dataInicio && i.Data_Indicacao <= dataFim)
                    .ToListAsync();

                var dados = indicacoes.GroupBy(i => new { i.Data_Indicacao.Year, i.Data_Indicacao.Month })
                    .Select(g => new {
                        Mes = $"{g.Key.Month:D2}/{g.Key.Year}",
                        TotalEnviadas = g.Count(),
                        TotalValidadas = g.Count(i => i.Status_Indicacao == SMRDominio.ClasseIndicacao.IndicacaoStatus.Validada),
                        Ano = g.Key.Year,
                        MesNum = g.Key.Month
                    }).OrderBy(m => m.Ano).ThenBy(m => m.MesNum).ToList();

                var documento = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.Header().Text("Métricas de Conversão").FontSize(20).Bold().FontColor("#D4AF37");

                        page.Content().Column(col =>
                        {
                            col.Spacing(10);
                            col.Item().Text($"Período: {dataInicio:dd/MM/yyyy} a {dataFim:dd/MM/yyyy}").FontSize(12);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background("#333333").Padding(5).Text("Mês/Ano").FontColor("#FFFFFF").Bold();
                                    header.Cell().Background("#333333").Padding(5).Text("Enviadas").FontColor("#FFFFFF").Bold().AlignCenter();
                                    header.Cell().Background("#333333").Padding(5).Text("Validadas").FontColor("#FFFFFF").Bold().AlignCenter();
                                    header.Cell().Background("#333333").Padding(5).Text("Conversão (%)").FontColor("#FFFFFF").Bold().AlignCenter();
                                });

                                foreach (var item in dados)
                                {
                                    double taxa = item.TotalEnviadas > 0 ? ((double)item.TotalValidadas / item.TotalEnviadas) * 100 : 0;

                                    table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(5).Text(item.Mes);
                                    table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(5).Text(item.TotalEnviadas.ToString()).AlignCenter();
                                    table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(5).Text(item.TotalValidadas.ToString()).AlignCenter();
                                    table.Cell().BorderBottom(1).BorderColor("#E0E0E0").Padding(5).Text($"{Math.Round(taxa, 2)}%").AlignCenter();
                                }
                            });
                        });

                        page.Footer().AlignCenter().Text(x => { x.Span("Página "); x.CurrentPageNumber(); });
                    });
                });

                return File(documento.GeneratePdf(), "application/pdf", $"Conversao_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao gerar PDF de Conversão: {ex.Message}" });
            }
        }
    }
}