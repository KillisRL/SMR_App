using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics.Text;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseIndicacao;
using SMRDominio.ClasseRecompensa;
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

        // O retorno da API agora é um objeto único (Wrapper), não uma lista de wrappers
        private ConsultaFinalHistorico _respostaTotal; 

        private List<IndicacaoHistoricoDto> _todasIndicacoes = new();
        private List<PromotorPontosResponse> _todosPontos = new();
        private List<RecompensaResponse> _recompensasEmpresa = new();

        private List<int> _idsEmpresasDisponiveis = new();
        private int _indexEmpresaAtual = 0;

        [ObservableProperty]
        private bool isFiltroAberto = false;

        [ObservableProperty] ObservableCollection<IndicacaoHistoricoDto> listaIndicacao = new();
        [ObservableProperty] private double progresso = 0;
        [ObservableProperty] private int pontosPromotor = 0;
        [ObservableProperty] private Recompensa_Rank promotorRank = Recompensa_Rank.ND;
        [ObservableProperty] private DateTime dataInicial = DateTime.Now.Date.AddDays(-7);
        [ObservableProperty] private DateTime dataFinal = DateTime.Now.Date.AddHours(23).AddMinutes(59);

        [ObservableProperty] private int idEmpresa;
        [ObservableProperty] private string nomeEmpresaAtual = "Carregando...";

        public HistIndicacoesEmpresaViewModel(ApiServiceIndicacao apiServiceIndicacao)
        {
            _apiServiceIndicacao = apiServiceIndicacao;
        }

        partial void OnIdEmpresaChanged(int value)
        {
            if (value > 0 && _todasIndicacoes.Any())
            {
                FiltrarIndicaçoesPorEmpresa();
            }
        }

        [RelayCommand]
        public void AbrirFiltro()
        {
            IsFiltroAberto = true;
        }
        [RelayCommand]
        public void FecharFiltro()
        {
            IsFiltroAberto = false;
        }


        [RelayCommand]
        public async Task SalvarFiltro()
        {
            IsFiltroAberto = false; 
            await IndicacaoConsultarHistorico();
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
                    _respostaTotal = resultado.Dados;

                    _todasIndicacoes = _respostaTotal.Indicacoes ?? new();
                    _todosPontos = _respostaTotal.PontosPromotor ?? new();
                    _recompensasEmpresa = _respostaTotal.RecompensasEmpresas ?? new();

                    // CORREÇÃO 1: Pega TODAS as empresas, mesmo as que ainda não têm indicações
                    _idsEmpresasDisponiveis = _todosPontos.Select(p => p.IDEmpresa)
                        .Union(_todasIndicacoes.Select(i => i.IdEmpresa))
                        .Union(_recompensasEmpresa.Select(r => r.IDEmpresa))
                        .Distinct()
                        .ToList();

                    if (_idsEmpresasDisponiveis.Contains(IdEmpresa))
                        _indexEmpresaAtual = _idsEmpresasDisponiveis.IndexOf(IdEmpresa);
                    else if (_idsEmpresasDisponiveis.Any())
                        IdEmpresa = _idsEmpresasDisponiveis.First();

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
            if (_idsEmpresasDisponiveis.Count <= 1) return;

            _indexEmpresaAtual++;
            if (_indexEmpresaAtual >= _idsEmpresasDisponiveis.Count)
                _indexEmpresaAtual = 0;

            IdEmpresa = _idsEmpresasDisponiveis[_indexEmpresaAtual];
        }

        [RelayCommand]
        public void EmpresaAnterior()
        {
            if (_idsEmpresasDisponiveis.Count <= 1) return;

            _indexEmpresaAtual--;
            if (_indexEmpresaAtual < 0)
                _indexEmpresaAtual = _idsEmpresasDisponiveis.Count - 1;

            IdEmpresa = _idsEmpresasDisponiveis[_indexEmpresaAtual];
        }

        [RelayCommand]
        public async Task AbrirDetalhesIndicacao(IndicacaoHistoricoDto indicacao)
        {
            if (indicacao == null)
            {
                await Shell.Current.DisplayAlert("Atenção", "Dados inválidos para consulta.", "Ok");
                return;
            }

            var parametro = new Dictionary<string, object>
            {
                {"CodigoIndicacao", indicacao.IDIndicacao}
            };

            await Shell.Current.GoToAsync(nameof(IndicacaoDetalhesView), parametro);
        }

        private void FiltrarIndicaçoesPorEmpresa()
        {
            var filtradas = _todasIndicacoes.Where(i => i.IdEmpresa == IdEmpresa).ToList();
            var dadosPromotor = _todosPontos.FirstOrDefault(p => p.IDEmpresa == IdEmpresa);

            // CORREÇÃO 2: Mostra o nome real da empresa mesmo se o histórico dela estiver vazio
            if (filtradas.Any())
                NomeEmpresaAtual = filtradas.First().RazaoSocial;
            else if (dadosPromotor != null)
                NomeEmpresaAtual = dadosPromotor.RazaoSocial;
            else
                NomeEmpresaAtual = "Empresa sem registros";

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // CORREÇÃO 3: Nunca use .Clear() aqui! Atribuição direta para blindar a CollectionView do Android
                ListaIndicacao = new ObservableCollection<IndicacaoHistoricoDto>(filtradas);

                if (dadosPromotor == null)
                {
                    PontosPromotor = 0;
                    PromotorRank = Recompensa_Rank.ND;
                    Progresso = 0;
                    return;
                }

                PontosPromotor = dadosPromotor.PontosAcumulados;
                PromotorRank = dadosPromotor.IDPromotorRank ?? Recompensa_Rank.ND;

                var rankAtual = _recompensasEmpresa.FirstOrDefault(r => r.IDEmpresa == IdEmpresa && r.IDRank == dadosPromotor.IDPromotorRank);

                if (rankAtual == null)
                {
                    var objetivo = _recompensasEmpresa.Where(r => r.IDEmpresa == IdEmpresa).OrderBy(r => r.PontosNecessarios).FirstOrDefault();
                    if (objetivo == null || objetivo.PontosNecessarios <= 0)
                        Progresso = 0;
                    else
                    {
                        double calc = (double)dadosPromotor.PontosAcumulados / objetivo.PontosNecessarios;
                        Progresso = Math.Clamp(calc, 0, 1);
                    }
                }
                else
                {
                    var objetivo = _recompensasEmpresa.Where(r => r.IDEmpresa == IdEmpresa && r.PontosNecessarios > dadosPromotor.PontosAcumulados).OrderBy(r => r.PontosNecessarios).FirstOrDefault();

                    if (objetivo == null)
                    {
                        Progresso = 1.0;
                    }
                    else
                    {
                        double pontosCalculados = (double)dadosPromotor.PontosAcumulados - rankAtual.PontosNecessarios;
                        double pontosObjetivo = (double)objetivo.PontosNecessarios - rankAtual.PontosNecessarios;

                        if (pontosObjetivo > 0)
                        {
                            double calc = pontosCalculados / pontosObjetivo;
                            Progresso = Math.Clamp(calc, 0, 1);
                        }
                        else
                            Progresso = 0;
                    }
                }
            });
        }
    }
}