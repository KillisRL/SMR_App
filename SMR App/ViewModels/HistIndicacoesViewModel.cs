using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClassePessoa;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    public partial class HistIndicacoesViewModel : BaseViewModel
    {
        private readonly ApiServicesPessoa _apiServicesPessoa;
        private readonly ApiServiceIndicacao _apiServiceIndicacao; // 1. Injeta o serviço de indicação

        [ObservableProperty] private string razaoSocial;
        [ObservableProperty] ObservableCollection<Empresa> listaEmpresa = new();

        public HistIndicacoesViewModel(ApiServicesPessoa apiServicesPessoa, ApiServiceIndicacao apiServiceIndicacao)
        {
            _apiServicesPessoa = apiServicesPessoa;
            _apiServiceIndicacao = apiServiceIndicacao;
            _ = ConsultarEmpresa();
        }

        [RelayCommand]
        public async Task AbrirHistorico(Empresa empresaSelecionada)
        {
            if (empresaSelecionada == null) return;

            var pessoaLogada = ApiServicesSessaoPessoa.PessoaLogada;
            string nomeUsuario = pessoaLogada != null ? pessoaLogada.nome : "Usuário";

            bool confirmacao = await Shell.Current.DisplayAlert(
                $"Olá, {nomeUsuario}",
                "O que deseja visualizar?",
                "Histórico de Indicação",
                "Resgate de Recompensa");

            await Task.Delay(150);

            var parametro = new Dictionary<string, object>
            {
                {"EmpresaIndicacao", empresaSelecionada.id },
                {"NomeEmpresaSelecionada", empresaSelecionada.razao_social }
            };
            if (confirmacao)
            {
                await Shell.Current.GoToAsync(nameof(HistIndicacoesEmpresaView), parametro);
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(HisIndicacoesResgateView), parametro);
            }
        }

        [RelayCommand]
        public async Task ConsultarEmpresa()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                // 1. Busca a lista completa de empresas (ou filtrada por razão social)
                var resultadoEmpresas = await _apiServicesPessoa.ConsultarEmpresa(token, RazaoSocial);

                // 2. Busca o histórico de indicações do promotor (janela ampla de datas)
                DateTime dataInicial = DateTime.Now.AddYears(-5);
                DateTime dataFinal = DateTime.Now.Date.AddHours(23).AddMinutes(59);
                var resultadoHistorico = await _apiServiceIndicacao.ConsultarIndicacaoHistorico(dataInicial, dataFinal, token);

                if (resultadoEmpresas.Sucesso && resultadoEmpresas.Dados != null)
                {
                    List<int> idsEmpresasComIndicacao = new();

                    // Extrai apenas os IDs das empresas onde o promotor REALMENTE possui indicações
                    if (resultadoHistorico.Sucesso && resultadoHistorico.Dados?.Indicacoes != null)
                    {
                        idsEmpresasComIndicacao = resultadoHistorico.Dados.Indicacoes
                            .Select(i => i.IdEmpresa)
                            .Distinct()
                            .ToList();
                    }

                    // 3. REGRA DE NEGÓCIO: Mantém na lista APENAS as empresas com indicação
                    var empresasFiltradas = resultadoEmpresas.Dados
                        .Where(e => idsEmpresasComIndicacao.Contains(e.id))
                        .ToList();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaEmpresa.Clear();
                        foreach (var item in empresasFiltradas)
                        {
                            ListaEmpresa.Add(item);
                        }
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(resultadoEmpresas.Mensagem) && resultadoEmpresas.Mensagem.Contains("Não foi encontrado"))
                    {
                        MainThread.BeginInvokeOnMainThread(() => ListaEmpresa.Clear());
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Atenção", resultadoEmpresas.Mensagem, "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível carregar as empresas. Erro: {ex.Message}", "OK");
            }
        }
    }
}