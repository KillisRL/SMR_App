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
        }

        [RelayCommand]
        public async Task AbrirHistorico(Empresa empresaSelecionada)
        {
            var parametro = new Dictionary<string, object>
            {
                {"EmpresaIndicacao", empresaSelecionada }
            };

            await Shell.Current.GoToAsync(nameof(IndicacaoEmpresaBonificacaoView), parametro);
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
