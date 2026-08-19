using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseIndicacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    [QueryProperty(nameof(CodigoIndicacao), "CodigoIndicacao")]
    public partial class IndicacaoDetalhesViewModel : BaseViewModel
    {
        private readonly ApiServiceIndicacao _apiServiceIndicacao;

        [ObservableProperty] private IndicacaoDetalhesDto indicacaoDetalhe;

        [ObservableProperty] private int codigoIndicacao;

        [ObservableProperty] private IndicacaoRetornoApiEnviada indicacaoEnviada;

        public IndicacaoDetalhesViewModel(ApiServiceIndicacao apiServiceIndicacao)
        {
            _apiServiceIndicacao = apiServiceIndicacao;
        }

        async partial void OnCodigoIndicacaoChanged(int value)
        {
            if (value > 0)
            {
                await IndicacaConsultarDetalhes();
            }
        }

        [RelayCommand]
        public async Task IndicacaoAlterarSituacao(IndicacaoStatus indicacaoStatus)
        {
            try
            {
                if (CodigoIndicacao <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Código da indicação inválido.", "Ok");
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceIndicacao.IndicacaoAlterarStatus(token, CodigoIndicacao, indicacaoStatus);

                if (resultado.Sucesso && resultado.Dados != null)
                {
                    IndicacaoEnviada = resultado.Dados;

                    // Se for envio (contém link/código), abre o compartilhador nativo
                    if (!string.IsNullOrEmpty(resultado.Dados.LinkValidacao) && !string.IsNullOrEmpty(resultado.Dados.CodigoValidacao))
                    {
                        await Application.Current.MainPage.DisplayAlert(
                            "Indicação Pronta!",
                            $"Código de validação gerado: {resultado.Dados.CodigoValidacao}\n\nVamos compartilhar o link com o indicado!",
                            "Compartilhar");

                        // Dispara o menu nativo (WhatsApp, SMS, Telegram, etc.)
                        await Share.Default.RequestAsync(new ShareTextRequest
                        {
                            Title = "Compartilhar Indicação",
                            Subject = "Seu Bônus Exclusivo!",
                            Text = $"Olá! Você recebeu uma indicação com bônus especial.\n\nApresente este código na empresa: {resultado.Dados.CodigoValidacao}\nOu acesse seu voucher pelo link: {resultado.Dados.LinkValidacao}"
                        });

                        // Atualiza os dados na tela para refletir o status novo
                        await IndicacaConsultarDetalhes();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                        await Shell.Current.GoToAsync(".."); // Retorna para a tela anterior
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Ocorreu uma falha: {ex.Message}", "Ok");
            }
        }


        [RelayCommand]
        public async Task IndicacaConsultarDetalhes()
         {
            try
            {
                if(CodigoIndicacao <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Não foi possível consultar os detalhes da indicação pois o código recebido é nulo","Ok");
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceIndicacao.ConsultarIndicacaoDetalhes(token, CodigoIndicacao);

                if(resultado.Sucesso && resultado.Dados != null)
                {
                    IndicacaoDetalhe = resultado.Dados;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Ateção", resultado.Mensagem, "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ateção", "Falha de comunicação com o servidor.", "Ok");
                return;
            }
        }

    }
}
