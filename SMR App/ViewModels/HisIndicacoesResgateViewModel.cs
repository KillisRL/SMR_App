using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMRDominio.ClasseBase;
using SMRDominio.ClasseIndicacao;
using SMRDominio.ClassePessoa;
using SMRDominio.ClasseRecompensa;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    [QueryProperty(nameof(IdEmpresa), "EmpresaIndicacao")]
    public partial class HisIndicacoesResgateViewModel : BaseViewModel
    {
        private readonly ApiServiceRecompensa _apiServiceRecompensa;
        private readonly ApiServiceIndicacao _apiServiceIndicacao;
        [ObservableProperty] private int idEmpresa;

        private ConsultaFinalResgate _respostaTotal;
        private List<IndicacaoRecompensaResgate> _todasRecompensas = new();
        private List<PromotorPontosResgate> _promotorTodosPontos = new();
        private List<RecompensaResgatadas> _recompensasResgatadas = new();
        private List<int> _idsEmpresas = new();
        private int _indexEmpresaAtual = 0;
        private int id_promotor = 0;
        public bool JaResgatada = false;
        public string NomeBotao => JaResgatada == false ? "RESGATAR!" : "RESGATADA";

        [ObservableProperty] ObservableCollection<IndicacaoRecompensaResgate> listaRecompensa = new();
        [ObservableProperty] private int pontosPromotor = 0;
        [ObservableProperty] private string nomeEmpresaAtual = "Carregando...";
        [ObservableProperty] private Recompensa_Rank promotorRank = Recompensa_Rank.ND;

        public HisIndicacoesResgateViewModel(ApiServiceIndicacao apiServiceIndicacao, ApiServiceRecompensa apiServiceRecompensa)
        {
            _apiServiceIndicacao = apiServiceIndicacao;
            _apiServiceRecompensa = apiServiceRecompensa;
        }

        [RelayCommand]
        public async Task ResgatarRecompensa(IndicacaoRecompensaResgate recompensa)
        {
            try
            {
                if(recompensa == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Não foi possível identificar a recompensa", "Ok");
                    return;
                }


                string? token = await SecureStorage.Default.GetAsync("jwt_token");

                var novoResgate = new RecompensaResgate
                {
                    id_empresa = IdEmpresa,
                    id_recompensa = recompensa.IDRecompensa,
                    data_resgate = DateTime.Now,
                    id_promotor = id_promotor
                };

                var resultado = await _apiServiceRecompensa.RecompensaResgatar(token, novoResgate);

                if(resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                    IndicacaoRecompensaResgate();
                    return;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível resgatar a recompensa selecionada. Erro: {ex.Message}", "OK");
            }
        }

        partial void OnIdEmpresaChanged(int value)
        {
            if (value > 0 && _todasRecompensas.Any())
            {
                FiltrarRecompensaPorEmpresas();
            }
        }
        [RelayCommand]
        public async Task IndicacaoRecompensaResgate()
        {
            try
            {
                string? token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceIndicacao.ConsultarRecompensaResgate(token);

                if (resultado.Sucesso && resultado.Dados != null)
                {
                    _respostaTotal = resultado.Dados;

                    _promotorTodosPontos = _respostaTotal.PromotorPontos ?? new();
                    _todasRecompensas = _respostaTotal.ListaRecompensas ?? new();
                    _recompensasResgatadas = _respostaTotal.Resgatadas ?? new();

                    _idsEmpresas = _todasRecompensas.Select(r => r.IDEmpresa).Distinct().ToList();

                    id_promotor = _promotorTodosPontos.Select(ptp => ptp.IDPromotor).FirstOrDefault();

                    if (_idsEmpresas.Contains(IdEmpresa))
                    {
                        _indexEmpresaAtual = _idsEmpresas.IndexOf(IdEmpresa);
                    }
                    else if (_idsEmpresas.Any())
                    {
                        // Se por acaso vier zerado, assume a primeira empresa da lista
                        IdEmpresa = _idsEmpresas.First();
                    }

                    FiltrarRecompensaPorEmpresas();
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
            if (_idsEmpresas.Count == 0) return;

            _indexEmpresaAtual++;
            if (_indexEmpresaAtual >= _idsEmpresas.Count)
                _indexEmpresaAtual = 0; // Volta para a primeira

            IdEmpresa = _idsEmpresas[_indexEmpresaAtual];
        }

        [RelayCommand]
        public void EmpresaAnterior()
        {
            if (_idsEmpresas.Count == 0) return;

            _indexEmpresaAtual--;
            if (_indexEmpresaAtual < 0)
                _indexEmpresaAtual = _idsEmpresas.Count - 1; // Vai para a última

            IdEmpresa = _idsEmpresas[_indexEmpresaAtual];
        }

        public void FiltrarRecompensaPorEmpresas()
        {
            var dadosPromotor = _promotorTodosPontos.FirstOrDefault(pontos => pontos.IDEmpresa == IdEmpresa);
            int pontosAtuais = dadosPromotor != null ? dadosPromotor.PontosAcumulados : 0;
            var resgatadasEmpresa = _recompensasResgatadas.Where(resgatada => resgatada.IDEmpresa == IdEmpresa); 

            PontosPromotor = pontosAtuais;
            PromotorRank = dadosPromotor != null ? dadosPromotor.IDPromotorRank ?? Recompensa_Rank.ND : Recompensa_Rank.ND;
            var filtradasAsc = _todasRecompensas
                    .Where(r => r.IDEmpresa == IdEmpresa)
                    .OrderBy(r => r.PontosNecessarios)
                    .ToList();

            if (filtradasAsc.Any())
                NomeEmpresaAtual = filtradasAsc.First().RazaoSocial;
            else
                NomeEmpresaAtual = "Nenhuma empresa selecionada";

            int pontosAnteriores = 0; 

            foreach (var item in filtradasAsc)
            {
                item.PodeResgatar = pontosAtuais >= item.PontosNecessarios;

                item.JaResgatada = _recompensasResgatadas.Any(resgatada =>
                                    resgatada.IDRecompensa == item.IDRecompensa &&
                                    resgatada.IDEmpresa == IdEmpresa);
                if (item.JaResgatada)
                {
                    item.PodeResgatar = false;
                }

                int pontosDesteRank = item.PontosNecessarios - pontosAnteriores;

                int pontosConquistadosNesteRank = Math.Max(0, pontosAtuais - pontosAnteriores);

                double percentual = (double)pontosConquistadosNesteRank / pontosDesteRank;
                if (percentual > 1.0) percentual = 1.0;

                item.AlturaPreenchimento = 160 * percentual;

                pontosAnteriores = item.PontosNecessarios;
            }
            var filtradasDesc = filtradasAsc.OrderByDescending(r => r.PontosNecessarios).ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaRecompensa.Clear();
                foreach (var item in filtradasDesc)
                {
                    ListaRecompensa.Add(item);
                }
            });
        }

    }
}
