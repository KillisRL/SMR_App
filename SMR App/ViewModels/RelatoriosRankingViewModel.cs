using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SMR_App.Services;
using SMRDominio.DTOs;
using System.Collections.ObjectModel;

namespace SMR_App.ViewModels
{
    public partial class RelatoriosRankingViewModel : BaseViewModel
    {
        private readonly RelatorioApiService _apiService;

        [ObservableProperty]
        private DateTime _dataInicio = new DateTime(2026, 8, 1);

        [ObservableProperty]
        private DateTime _dataFim = new DateTime(2026, 8, 31);

        [ObservableProperty]
        private StatusFiltro _statusSelecionado;

        public ObservableCollection<StatusFiltro> ListaStatus { get; }

        [ObservableProperty]
        private ISeries[]? _seriesGrafico;

        [ObservableProperty]
        private Axis[]? _eixosX;

        [ObservableProperty]
        private Axis[]? _eixosY;

        public RelatoriosRankingViewModel(RelatorioApiService apiService)
        {
            _apiService = apiService;
            NomeUsuario = "Ueler Bernardo";

            // Monta a lista de filtros
            ListaStatus = new ObservableCollection<StatusFiltro>
            {
                new StatusFiltro { Id = 0, Descricao = "Todas" },
                new StatusFiltro { Id = 1, Descricao = "Pendentes" },
                new StatusFiltro { Id = 2, Descricao = "Enviadas" },
                new StatusFiltro { Id = 3, Descricao = "Canceladas" },
                new StatusFiltro { Id = 4, Descricao = "Validadas" }
            };

            // Inicia em 'Todas'
            StatusSelecionado = ListaStatus.First();

            ConfigurarEixosIniciais();
            _ = CarregarDadosGraficoAsync();
        }

        private void ConfigurarEixosIniciais()
        {
            EixosY = new Axis[]
            {
                new Axis
                {
                    MinStep = 1,
                    MinLimit = 0,
                    TextSize = 12
                }
            };
        }

        [RelayCommand]
        private async Task VoltarInicio() => await Shell.Current.GoToAsync("//MainPage");

        [RelayCommand]
        private void RelatorioAnterior() { }

        [RelayCommand]
        private void ProximoRelatorio() { }

        // Eventos disparados ao mudar os filtros
        partial void OnDataInicioChanged(DateTime value) => _ = CarregarDadosGraficoAsync();
        partial void OnDataFimChanged(DateTime value) => _ = CarregarDadosGraficoAsync();
        partial void OnStatusSelecionadoChanged(StatusFiltro value) => _ = CarregarDadosGraficoAsync();

        private async Task CarregarDadosGraficoAsync()
        {
            if (StatusSelecionado == null) return;

            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token)) return;

                var dados = await _apiService.ObterRankingPromotoresAsync(DataInicio, DataFim, StatusSelecionado.Id, token);

                if (dados == null || !dados.Any())
                {
                    SeriesGrafico = Array.Empty<ISeries>();
                    EixosX = Array.Empty<Axis>();
                    return;
                }

                var dadosOrdenados = dados.OrderBy(d => d.Quantidade).ToList();
                var valores = dadosOrdenados.Select(d => (double)d.Quantidade).ToArray();
                var nomes = dadosOrdenados.Select(d => d.NomePromotor ?? "").ToArray();

                SeriesGrafico = new ISeries[]
                {
                    new ColumnSeries<double>
                    {
                        Values = valores,
                        Name = "Indicações",
                        Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                        MaxBarWidth = 40
                    }
                };

                EixosX = new Axis[]
                {
                    new Axis
                    {
                        Labels = nomes,
                        TextSize = 12,
                        LabelsRotation = 15
                    }
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar ranking: {ex.Message}");
            }
        }
    }

    // Classe auxiliar para o Picker da tela
    public class StatusFiltro
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
    }
}