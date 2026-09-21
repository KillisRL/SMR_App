using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClassePessoa;
using System.Collections.ObjectModel;

namespace SMR_App.ViewModels
{
    public partial class HistIndicacoesViewModel : BaseViewModel
    {
        private readonly ApiServicesPessoa _apiServicesPessoa;

        [ObservableProperty] private string razaoSocial;
        [ObservableProperty] ObservableCollection<Empresa> listaEmpresa = new();


        public HistIndicacoesViewModel(ApiServicesPessoa apiServicesPessoa)
        {
            _apiServicesPessoa = apiServicesPessoa;
            _ = ConsultarEmpresa();
        }

        [RelayCommand]
        public async Task AbrirHistorico(Empresa empresaSelecionada)
        {
            if (empresaSelecionada == null) return;

            var pessoaLogada = ApiServicesSessaoPessoa.PessoaLogada;
            string nomeUsuario = pessoaLogada != null ? pessoaLogada.nome : "Usuário";

            // Trocado para Shell.Current (mais seguro para navegação e alertas)
            bool confirmacao = await Shell.Current.DisplayAlert(
                $"Olá, {nomeUsuario}",
                "O que deseja visualizar?",
                "Histórico de Indicação",
                "Resgate de Recompensa");

            await Task.Delay(150);

            var parametro = new Dictionary<string, object>
            {
                {"EmpresaIndicacao", empresaSelecionada.id }
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

                var resultado = await _apiServicesPessoa.ConsultarEmpresa(token, RazaoSocial);

                if (resultado.Sucesso && resultado.Dados != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaEmpresa.Clear();
                        foreach (var item in resultado.Dados)
                        {
                            ListaEmpresa.Add(item);
                        }
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(resultado.Mensagem) && resultado.Mensagem.Contains("Não foi encontrado"))
                    {
                        MainThread.BeginInvokeOnMainThread(() => ListaEmpresa.Clear());
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
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
