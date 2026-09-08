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

                // Atenção: Certifique-se de que o método ConsultarIndicacaoHistorico no seu ApiService 
                // esteja tipado para retornar ConsultarFinal (o wrapper) dentro do 'Dados'
                var resultado = await _apiServiceIndicacao.ConsultarIndicacaoHistorico(DataInicial, DataFinal, token);

                if (resultado.Sucesso && resultado.Dados != null)
                {
                    _respostaTotal = resultado.Dados;

                    // Desempacotando as listas do Wrapper
                    _todasIndicacoes = _respostaTotal.Indicacoes ?? new();
                    _todosPontos = _respostaTotal.PontosPromotor ?? new();
                    _recompensasEmpresa = _respostaTotal.RecompensasEmpresas ?? new();

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

                var dadosPromotor = _todosPontos.FirstOrDefault(p => p.IDEmpresa == IdEmpresa);

                // Proteção contra NullReferenceException
                if (dadosPromotor == null)
                {
                    PontosPromotor = 0;
                    PromotorRank = Recompensa_Rank.ND;
                    Progresso = 0;
                    return; // Encerra a lógica aqui, pois não há o que calcular
                }

                PontosPromotor = dadosPromotor.PontosAcumulados;
                PromotorRank = dadosPromotor.IDPromotorRank ?? Recompensa_Rank.ND;

                var rankAtual = _recompensasEmpresa.FirstOrDefault(r => r.IDEmpresa == IdEmpresa && r.IDRank == dadosPromotor.IDPromotorRank);

                if (rankAtual == null)
                {
                    var objetivo = _recompensasEmpresa.Where(r => r.IDEmpresa == IdEmpresa)
                                                      .OrderBy(r => r.PontosNecessarios)
                                                      .FirstOrDefault();
                    if (objetivo == null)
                    {
                        Progresso = 0;
                    }
                    else
                    {
                        Progresso = (double)dadosPromotor.PontosAcumulados / objetivo.PontosNecessarios;
                    }
                }
                else
                {
                    var objetivo = _recompensasEmpresa
                                        .Where(r => r.IDEmpresa == IdEmpresa && r.PontosNecessarios > dadosPromotor.PontosAcumulados)
                                        .OrderBy(r => r.PontosNecessarios)
                                        .FirstOrDefault();

                    if (objetivo == null)
                    {
                        // Usuário atingiu o Rank Máximo! A barra deve ficar 100% cheia.
                        Progresso = 1.0;
                    }
                    else
                    {
                        // Progresso Relativo: Usa as variáveis que você mesmo criou para mostrar o avanço DENTRO do nível atual
                        double pontosCalculados = (double)dadosPromotor.PontosAcumulados - rankAtual.PontosNecessarios;
                        double pontosObjetivo = (double)objetivo.PontosNecessarios - rankAtual.PontosNecessarios;

                        // Evita divisão por zero caso a regra de negócio tenha cadastrado recompensas com a mesma pontuação
                        if (pontosObjetivo > 0)
                        {
                            Progresso = pontosCalculados / pontosObjetivo;
                        }
                        else
                        {
                            Progresso = 0;
                        }
                    }
                }
            });
        }
    }
}