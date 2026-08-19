using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClassePessoa;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    public partial class IndicacaoConsultarEmpresaViewModel : BaseViewModel
    {
        private readonly ApiServicesPessoa _apiServicesPessoa;

        [ObservableProperty] private ObservableCollection<Empresa> listaEmpresa = new();

        // Inicializa vazia para evitar NullReference na API
        [ObservableProperty] private string razaoSocial = string.Empty;

        public IndicacaoConsultarEmpresaViewModel(ApiServicesPessoa apiServicesPessoa)
        {
            _apiServicesPessoa = apiServicesPessoa;
        }

        [RelayCommand]
        public async Task AbrirCadastro(Empresa empresaSelecionada)
        {
            bool confirmacao = await Application.Current.MainPage.DisplayAlert("Atenção", $"Deseja realizar indicação para empresa \"{empresaSelecionada.razao_social}\"?","Sim", "Não");

            if(!confirmacao)
            {
                return;
            }

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