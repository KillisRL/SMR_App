using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseIndicacao;
using System;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    [QueryProperty(nameof(CodigoIndicacao), "CodigoIndicacao")]
    [QueryProperty(nameof(DadosValidacao), "DadosValidacao")]
    public partial class IndicacaoDetalhesViewModel : BaseViewModel
    {
        private readonly ApiServiceIndicacao _apiServiceIndicacao;

        [ObservableProperty] private IndicacaoDetalhesDto indicacaoDetalhe;
        [ObservableProperty] private int codigoIndicacao;
        [ObservableProperty] private int dadosValidacao;

        [ObservableProperty] private bool podeVisualizar;
        [ObservableProperty] private bool podeVisualizarValidacao;

        public IndicacaoDetalhesViewModel(ApiServiceIndicacao apiServiceIndicacao)
        {
            _apiServiceIndicacao = apiServiceIndicacao;
        }

        // 🚀 Disparado automaticamente se vier "CodigoIndicacao"
        partial void OnCodigoIndicacaoChanged(int value)
        {
            if (value > 0)
            {
                VerificarEProcessarCarregamento();
            }
        }

        // 🚀 Disparado automaticamente se vier "DadosValidacao" (vindo do QR Code)
        partial void OnDadosValidacaoChanged(int value)
        {
            if (value > 0)
            {
                VerificarEProcessarCarregamento();
            }
        }

        // Propriedade auxiliar para pegar o ID real independentemente de onde veio
        private int ObterIdAtivo() => DadosValidacao > 0 ? DadosValidacao : CodigoIndicacao;

        private void VerificarEProcessarCarregamento()
        {
            DefinirModoTela();
            _ = IndicacaConsultarDetalhes(); // Executa a consulta em segundo plano
        }

        public void DefinirModoTela()
        {
            // Se DadosValidacao for maior que 0, significa que a empresa abriu via QR Code para validar
            if (DadosValidacao > 0)
            {
                PodeVisualizar = false;          // Esconde campos específicos do promotor
                PodeVisualizarValidacao = true;  // Exibe o botão de confirmação para a empresa
            }
            else
            {
                PodeVisualizar = true;           // Visão normal do promotor
                PodeVisualizarValidacao = false; // Oculta o botão da empresa
            }
        }

        [RelayCommand]
        public async Task ConfirmarValidacaoEmpresa()
        {
            try
            {
                int idAtivo = ObterIdAtivo();
                if (idAtivo <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Código da indicação inválido.", "Ok");
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceIndicacao.IndicacaoAlterarStatus(token, idAtivo, IndicacaoStatus.Validada);

                if (resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", "Indicação validada com sucesso! Bônus liberado.", "Ok");
                    await Shell.Current.GoToAsync(".."); // Retorna para a tela anterior
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao validar: {ex.Message}", "Ok");
            }
        }

        [RelayCommand]
        public async Task IndicacaConsultarDetalhes()
        {
            try
            {
                int idAtivo = ObterIdAtivo();
                if (idAtivo <= 0)
                {
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceIndicacao.ConsultarIndicacaoDetalhes(token, idAtivo);

                if (resultado.Sucesso && resultado.Dados != null)
                {
                    IndicacaoDetalhe = resultado.Dados;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
                }
            }
            catch (Exception)
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Falha de comunicação com o servidor.", "Ok");
            }
        }
    }
}