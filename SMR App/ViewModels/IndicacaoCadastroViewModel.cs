using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseBonificacao;
using SMRDominio.ClasseIndicacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    [QueryProperty(nameof(BonificacaoRecebida), ("BonificacaoEnviada"))]
    public partial class IndicacaoCadastroViewModel : BaseViewModel
    {
        private readonly ApiServiceIndicacao _apiServiceIndicacao;
        [ObservableProperty] private string nome_Indicado;
        [ObservableProperty] private string telefone_Indicado;
        //[ObservableProperty] private IndicacaoStatus status_Indicacao;
        [ObservableProperty] private DateTime data_Indicacao;
        [ObservableProperty] private DateTime data_Validacao;
        //[ObservableProperty] private int id_Bonificacao;
        [ObservableProperty] private string cpf;
        [ObservableProperty] private Bonificacao bonificacaoRecebida;

        public IndicacaoCadastroViewModel(ApiServiceIndicacao apiServiceIndicacao)
        {
            _apiServiceIndicacao = apiServiceIndicacao;
        }



        [RelayCommand]
        public async Task CadastrarIndicacao()
        {
            try
            { 

                if(string.IsNullOrEmpty(Cpf) || string.IsNullOrEmpty(Nome_Indicado) || string.IsNullOrEmpty(Telefone_Indicado))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha todos os campos para concluir o cadastro","Ok");
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var novaIndicacao = new Indicacao
                {
                    CPF = Cpf,
                    Data_Validacao = null,
                    Data_Indicacao = DateTime.Now,
                    Nome_Indicado = Nome_Indicado,
                    Telefone_Indicado = Telefone_Indicado,
                    Id_Bonificacao = BonificacaoRecebida.Id,
                    Status_Indicacao = IndicacaoStatus.Pendente
                };

                var resultado = await _apiServiceIndicacao.CadastrarIndicacao(token, novaIndicacao);

                if(resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");

                    var parametro = new Dictionary<string, object>
                    {
                        {"CodigoIndicacao", resultado.codigoIndicacao }
                    };

                    await Shell.Current.GoToAsync(nameof(IndicacaoDetalhesView), parametro);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Falha", resultado.Mensagem, "Ok");
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
