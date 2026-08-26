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
        partial void OnCodigoIndicacaoChanged(int value)
        {
            if (value > 0)
            {
                VerificarEProcessarCarregamento();
            }
        }
        partial void OnDadosValidacaoChanged(int value)
        {
            if (value > 0)
            {
                VerificarEProcessarCarregamento();
            }
        }
        private int ObterIdAtivo() => DadosValidacao > 0 ? DadosValidacao : CodigoIndicacao;

        private void VerificarEProcessarCarregamento()
        {
            DefinirModoTela();
            _ = IndicacaConsultarDetalhes();
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
        public async Task IndicacaoAlterarSituacao(IndicacaoStatus indicacaoStatus)
        {
            try
            {
                if (CodigoIndicacao <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Código da indicação inválido.", "Ok");
                    return;
                }

                if (indicacaoStatus == IndicacaoStatus.Enviada)
                {
                    bool confirmar = await Application.Current.MainPage.DisplayAlert("Atenção", "Deseja realmente enviar a indicação para empresa?", "Sim", "Não");
                    if (!confirmar)
                    {
                        return;
                    }
                }
                else if (indicacaoStatus == IndicacaoStatus.Cancelada)
                {
                    bool confirmar = await Application.Current.MainPage.DisplayAlert("Atenção", "Deseja realmente cancelar a indição?", "Sim", "Não");
                    if (!confirmar)
                    {
                        return;
                    }
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceIndicacao.IndicacaoAlterarStatus(token, CodigoIndicacao, indicacaoStatus);

                if (resultado.Sucesso && resultado.Dados != null)
                {
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
                        await Shell.Current.GoToAsync(nameof(PrincipalView));
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                        await Shell.Current.GoToAsync("..");
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