using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMRDominio.ClasseIndicacao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    [QueryProperty(nameof(IdEmpresa), "EmpresaIndicacao")]
    public partial class HistIndicacoesEmpresaViewModel : BaseViewModel
    {
        private readonly ApiServiceIndicacao _apiServiceIndicacao;

        // Lista completa que vem da API (todas as empresas)
        private List<IndicacaoHistoricoDto> _todasIndicacoes = new();

        private List<int> _idsEmpresasDisponiveis = new();
        private int _indexEmpresaAtual = 0;

        // Lista filtrada que será exibida na CollectionView da tela
        [ObservableProperty] ObservableCollection<IndicacaoHistoricoDto> listaIndicacao = new();

        [ObservableProperty] private DateTime dataInicial = DateTime.Now.Date.AddDays(-30);
        [ObservableProperty] private DateTime dataFinal = DateTime.Now.Date.AddHours(23).AddMinutes(59);

        [ObservableProperty] private int idEmpresa;
        [ObservableProperty] private string nomeEmpresaAtual = "Carregando...";

        public HistIndicacoesEmpresaViewModel(ApiServiceIndicacao apiServiceIndicacao)
        {
            _apiServiceIndicacao = apiServiceIndicacao;
        }

        // Este método roda automaticamente assim que o [QueryProperty] preenche o IdEmpresa!
        partial void OnIdEmpresaChanged(int value)
        {
            if (value > 0 && _todasIndicacoes.Any())
            {
                FiltrarIndicaçoesPorEmpresa();
            }
        }

        [RelayCommand]
        public async Task IndicacaoConsultarHistorico()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceIndicacao.ConsultarIndicacaoHistorico(DataInicial, DataFinal, token);

                if (resultado.Sucesso && resultado.Dados != null)
                {
                    _todasIndicacoes = resultado.Dados;

                    // Extrai as empresas únicas disponíveis para o carrossel
                    _idsEmpresasDisponiveis = _todasIndicacoes.Select(i => i.IdEmpresa).Distinct().ToList();

                    // Se o IdEmpresa atual veio da tela anterior, acha o índice dele
                    if (_idsEmpresasDisponiveis.Contains(IdEmpresa))
                    {
                        _indexEmpresaAtual = _idsEmpresasDisponiveis.IndexOf(IdEmpresa);
                    }
                    else if (_idsEmpresasDisponiveis.Any())
                    {
                        // Se por acaso vier zerado, assume a primeira empresa da lista
                        IdEmpresa = _idsEmpresasDisponiveis.First();
                    }

                    FiltrarIndicaçoesPorEmpresa();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível carregar o histórico. Erro: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public void ProximaEmpresa()
        {
            if (_idsEmpresasDisponiveis.Count == 0) return;

            _indexEmpresaAtual++;
            if (_indexEmpresaAtual >= _idsEmpresasDisponiveis.Count)
                _indexEmpresaAtual = 0; // Volta para a primeira

            IdEmpresa = _idsEmpresasDisponiveis[_indexEmpresaAtual];
        }

        [RelayCommand]
        public void EmpresaAnterior()
        {
            if (_idsEmpresasDisponiveis.Count == 0) return;

            _indexEmpresaAtual--;
            if (_indexEmpresaAtual < 0)
                _indexEmpresaAtual = _idsEmpresasDisponiveis.Count - 1; // Vai para a última

            IdEmpresa = _idsEmpresasDisponiveis[_indexEmpresaAtual];
        }

        // Método que filtra a lista baseada na empresa atual
        private void FiltrarIndicaçoesPorEmpresa()
        {
            var filtradas = _todasIndicacoes.Where(i => i.IdEmpresa == IdEmpresa).ToList();

            if (filtradas.Any())
            {
                NomeEmpresaAtual = filtradas.First().RazaoSocial;
            }
            else
            {
                NomeEmpresaAtual = "Nenhuma empresa selecionada";
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaIndicacao.Clear();
                foreach (var item in filtradas)
                {
                    ListaIndicacao.Add(item);
                }
            });
        }
    }
}