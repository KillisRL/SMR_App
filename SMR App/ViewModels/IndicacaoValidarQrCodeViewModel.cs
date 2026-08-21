using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using SMR_App.Views;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    public partial class IndicacaoValidarQrCodeViewModel : BaseViewModel
    {
        private readonly ApiServiceIndicacao _apiServiceIndicacao;

        [ObservableProperty] private string? codigoValidacao;

        [ObservableProperty]
        private bool isDetecting = true;

        [ObservableProperty] private bool isBusy;

        public IndicacaoValidarQrCodeViewModel(ApiServiceIndicacao apiServiceIndicacao)
        {
            _apiServiceIndicacao = apiServiceIndicacao;
        }

        [RelayCommand]
        public async Task IndicacaoConsultarCodigo()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceIndicacao.ConsultarIndicacaoValidacao(token, codigoValidacao);

                if (resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Dados.Mensagem, "Ok");

                    var parametro = new Dictionary<string, object>
                    {
                        {"DadosValidacao", resultado.Dados.IDIndicacao}
                    };

                    await Shell.Current.GoToAsync(nameof(IndicacaoDetalhesView), parametro);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Dados.Mensagem, "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao validar: {ex.Message}", "Ok");
                IsDetecting = true;
            }
        }


        [RelayCommand]
        public async Task ProcessarIndicacaoCodigo()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                if (string.IsNullOrEmpty(CodigoValidacao))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha o código para relaizar a validação da indicação", "Ok");
                    return;
                }


                var resultado = await _apiServiceIndicacao.ConfirmarValidacaoPorCodigo(token, CodigoValidacao);

                if (resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                    await Shell.Current.GoToAsync(".."); // Volta de tela
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Tentar Novamente");
                    IsDetecting = true; // Reativa a câmera para nova leitura
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao validar: {ex.Message}", "Ok");
                IsDetecting = true;
            }
        }

        public async Task ProcessarQrCodeLido(string valorQrCode)
        {
            // Pausa a câmera para não disparar várias vezes seguidas
            IsDetecting = false;
            IsBusy = true;

            try
            {
                // Se o QR Code leu uma URL inteira (ex: https://site.com/validar/K8M2P9XA), extrai apenas o código
                string codigo = valorQrCode.Trim();
                if (codigo.Contains("/validar/"))
                {
                    codigo = codigo.Substring(codigo.LastIndexOf('/') + 1);
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                // Chama o endpoint seguro da API para a empresa validar
                var resultado = await _apiServiceIndicacao.ConfirmarValidacaoPorCodigo(token, codigo);

                if (resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                    await Shell.Current.GoToAsync(".."); // Volta de tela
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Tentar Novamente");
                    IsDetecting = true; // Reativa a câmera para nova leitura
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao validar: {ex.Message}", "Ok");
                IsDetecting = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task Cancelar()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
