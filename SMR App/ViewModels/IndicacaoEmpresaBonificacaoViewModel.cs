using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseBonificacao;
using SMRDominio.ClassePessoa;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    [QueryProperty (nameof(EmpresaIndicacao), "EmpresaIndicacao")]
   public partial class IndicacaoEmpresaBonificacaoViewModel : BaseViewModel
    {
        private readonly ApiServicesBonificacao _apiServicesBonificacao;
        [ObservableProperty] private ObservableCollection<Bonificacao> listaBonificacao = new();

        [ObservableProperty] private Empresa empresaIndicacao;

        public IndicacaoEmpresaBonificacaoViewModel(ApiServicesBonificacao apiServicesBonificacao)
        {
            _apiServicesBonificacao = apiServicesBonificacao;
            _ = ConsultarBonificacaoEmpresa();
        }

        [RelayCommand]
        public async Task AbrirCadastroIndicacao(Bonificacao bonificacao)
        {
            if(bonificacao == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Dados inválido para o cadastro da bonificação.", "Ok");
                return;
            }
            var parametro = new Dictionary<string, object>
            {
                {"BonificacaoEnviada", bonificacao }
            };

            await Shell.Current.GoToAsync(nameof(IndicacaoCadastroView), parametro);

        }


        [RelayCommand]
        public async Task ConsultarBonificacaoEmpresa()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServicesBonificacao.ConsultarBonificacaoIndicacao(token, EmpresaIndicacao.id);

                if (resultado.Sucesso && resultado.Dados != null)
                {
                    ListaBonificacao.Clear();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var item in resultado.Dados)
                        {
                            ListaBonificacao.Add(item);
                        }
                    });
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível carregar as bonificações. Erro: {ex.Message}", "OK");
            }
        }
    }
}
