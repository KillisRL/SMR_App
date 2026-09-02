using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SMR_App.Services;
using SMRDominio.DTOs;

namespace SMR_App.ViewModels
{
    public partial class RelatoriosCoBonifViewModel : BaseViewModel
    {
        private readonly RelatorioApiService _apiService;

        [ObservableProperty]
        private string _nomeUsuario = "Ueler Bernardo";

        [ObservableProperty]
        private DateTime _dataInicio = new DateTime(2026, 8, 1);

        [ObservableProperty]
        private DateTime _dataFim = new DateTime(2026, 8, 31);

        [ObservableProperty]
        private ISeries[]? _seriesGrafico;

        [ObservableProperty]
        private Axis[]? _eixosX;

        [ObservableProperty]
        private Axis[]? _eixosY;

        public RelatoriosCoBonifViewModel()
        {
            _apiService = new RelatorioApiService();

            ConfigurarEixosIniciais();
            _ = CarregarDadosGraficoAsync();
        }

        private void ConfigurarEixosIniciais()
        {
            EixosY = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"R$ {value:N2}",
                    TextSize = 12
                }
            };
        }

        [RelayCommand]
        private async Task VoltarInicio()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        private void RelatorioAnterior() { }

        [RelayCommand]
        private void ProximoRelatorio() { }

        // Disparados automaticamente quando o usuário altera a data no DatePicker
        partial void OnDataInicioChanged(DateTime value) => _ = CarregarDadosGraficoAsync();
        partial void OnDataFimChanged(DateTime value) => _ = CarregarDadosGraficoAsync();

        private async Task CarregarDadosGraficoAsync()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                if (string.IsNullOrEmpty(token))
                {
                    // Caso o token tenha expirado ou não exista
                    return;
                }

                // O controller da API agora cuida de identificar a empresa logada através do token!
                var dados = await _apiService.ObterCustoIndicacaoAsync(DataInicio, DataFim, token);

                if (dados == null || !dados.Any())
                {
                    await Application.Current.MainPage.DisplayAlert("Sem Dados", "Nenhuma bonificação encontrada neste período.", "OK");

                    SeriesGrafico = Array.Empty<ISeries>();
                    EixosX = Array.Empty<Axis>();
                    return;
                }

                var valores = dados.Select(d => d.ValorCusto).ToArray();
                var meses = dados.Select(d => d.Mes ?? "").ToArray();

                SeriesGrafico = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = valores,
                        Name = "Custo Indicação",
                        Fill = null,
                        GeometrySize = 10,
                        Stroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 3 },
                        GeometryStroke = new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 3 }
                    }
                };

                EixosX = new Axis[]
                {
                    new Axis
                    {
                        Labels = meses,
                        TextSize = 12
                    }
                };
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Falha ao carregar gráfico: {ex.Message}", "OK");
            }
        }
    }
}