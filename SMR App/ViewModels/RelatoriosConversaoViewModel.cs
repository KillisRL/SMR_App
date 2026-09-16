using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SMR_App.Services;
using System.Collections.ObjectModel;

namespace SMR_App.ViewModels
{
    public partial class RelatoriosConversaoViewModel : BaseViewModel
    {
        private readonly RelatorioApiService _apiService;

        [ObservableProperty]
        private DateTime _dataInicio = new DateTime(DateTime.Now.Year, 1, 1); // Início do ano

        [ObservableProperty]
        private DateTime _dataFim = DateTime.Now;

        [ObservableProperty]
        private ISeries[]? _seriesGrafico;

        [ObservableProperty]
        private Axis[]? _eixosX;

        [ObservableProperty]
        private Axis[]? _eixosY;

        public RelatoriosConversaoViewModel(RelatorioApiService apiService)
        {
            _apiService = apiService;
            ConfigurarEixosIniciais();
            _ = CarregarDadosGraficoAsync();
        }

        private void ConfigurarEixosIniciais()
        {
            // Configura o eixo Y para exibir % e travar entre 0 e 100
            EixosY = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 100, // Limite máximo para porcentagem
                    Labeler = value => $"{value:N0}%", // Adiciona o símbolo %
                    TextSize = 12
                }
            };
        }

        // Navegação
        [RelayCommand] private void RelatorioAnterior() { /* Navegar para Relatorio Ranking */ }
        [RelayCommand] private void ProximoRelatorio() { /* Navegar para Relatorio Custo */ }

        // Gatilhos para recarregar ao mudar a data
        partial void OnDataInicioChanged(DateTime value) => _ = CarregarDadosGraficoAsync();
        partial void OnDataFimChanged(DateTime value) => _ = CarregarDadosGraficoAsync();

        private async Task CarregarDadosGraficoAsync()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token)) return;

                // Aqui você chamará o serviço da API para buscar a lista
                // DTO sugerido: { string Mes, double TaxaConversao }
                var dados = await _apiService.ObterMeticasConversaoAsync(DataInicio, DataFim, token);

                if (dados == null || !dados.Any())
                {
                    SeriesGrafico = Array.Empty<ISeries>();
                    EixosX = Array.Empty<Axis>();
                    return;
                }

                var valores = dados.Select(d => d.TaxaConversao).ToArray();
                var meses = dados.Select(d => d.Mes ?? "").ToArray();

                SeriesGrafico = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = valores,
                        Name = "Conversão",
                        Fill = null, // Deixa a linha sem preenchimento sólido em baixo
                        GeometrySize = 10,
                        // Usando o Dourado do seu app para a linha
                        Stroke = new SolidColorPaint(SKColor.Parse("#D4AF37")) { StrokeThickness = 3 },
                        GeometryStroke = new SolidColorPaint(SKColor.Parse("#D4AF37")) { StrokeThickness = 3 }
                    }
                };

                EixosX = new Axis[]
                {
                    new Axis
                    {
                        Labels = meses,
                        TextSize = 12,
                        LabelsRotation = 0
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar conversão: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ExportarExcelAsync()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "Sessão expirada.", "OK");
                    return;
                }

                var bytes = await _apiService.BaixarConversaoExcelAsync(DataInicio, DataFim, token);

                if (bytes == null || bytes.Length == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível gerar o arquivo Excel de conversão.", "OK");
                    return;
                }

                string nomeArquivo = $"Metricas_Conversao_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                string caminhoArquivo = Path.Combine(FileSystem.CacheDirectory, nomeArquivo);
                await File.WriteAllBytesAsync(caminhoArquivo, bytes);

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(caminhoArquivo)
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Erro ao exportar Excel: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task ExportarPdfAsync()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "Sessão expirada.", "OK");
                    return;
                }

                var bytes = await _apiService.BaixarConversaoPdfAsync(DataInicio, DataFim, token);

                if (bytes == null || bytes.Length == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível gerar o arquivo PDF de conversão.", "OK");
                    return;
                }

                string nomeArquivo = $"Metricas_Conversao_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string caminhoArquivo = Path.Combine(FileSystem.CacheDirectory, nomeArquivo);
                await File.WriteAllBytesAsync(caminhoArquivo, bytes);

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(caminhoArquivo)
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Erro ao exportar PDF: {ex.Message}", "OK");
            }
        }
    }
}