using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SMR_App.Services;
using SMR_App.ViewModels;
using SMR_App.Views;
using ZXing.Net.Maui.Controls;

namespace SMR_App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseBarcodeReader()
                .UseSkiaSharp()
                .UseLiveCharts()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Adiciona a fábrica de HttpClient necessária para os serviços de API
            builder.Services.AddSingleton<HttpClient>();

            // Serviços
            //builder.Services.AddSingleton<ApiServicesSessaoPessoa>();
            builder.Services.AddSingleton<ApiServicesPessoa>();
            builder.Services.AddSingleton<ApiServicesBonificacao>();
            builder.Services.AddSingleton<ApiServiceRecompensa>();
            builder.Services.AddSingleton<ApiServiceIndicacao>();
            builder.Services.AddSingleton<RelatorioApiService>();

            // ViewModels
            builder.Services.AddTransient<PessoaViewModel>();
            builder.Services.AddTransient<PrincipalViewModel>();
            builder.Services.AddTransient<ConfigEmpresaViewModel>();
            builder.Services.AddTransient<BonificacoesViewModel>();
            builder.Services.AddTransient<GrBonificacoesViewModel>();
            builder.Services.AddTransient<RecompensasViewModel>();
            builder.Services.AddTransient<GrRecompensasViewModel>();
            builder.Services.AddTransient<RelatoriosViewModel>();
            builder.Services.AddTransient<IndicacoesViewModel>();
            builder.Services.AddTransient<ReSenhaViewModel>();
            builder.Services.AddTransient<CadBoniViewModel>();
            builder.Services.AddTransient<CadRecompensaViewModel>();
            builder.Services.AddTransient<IndicacaoConsultarEmpresaViewModel>();
            builder.Services.AddTransient<IndicacaoEmpresaBonificacaoViewModel>();
            builder.Services.AddTransient<IndicacaoCadastroViewModel>();
            builder.Services.AddTransient<IndicacaoDetalhesViewModel>();
            builder.Services.AddTransient<IndicacaoValidarQrCodeViewModel>();
            builder.Services.AddTransient<HistIndicacoesViewModel>();
            builder.Services.AddTransient<HistIndicacoesEmpresaViewModel>();
            builder.Services.AddTransient<RelatoriosCoBonifViewModel>();
            builder.Services.AddTransient<RelatoriosRankingViewModel>();



            // Views
            builder.Services.AddTransient<CadastroPessoaView>();
            builder.Services.AddTransient<LoginView>();
            builder.Services.AddTransient<PrincipalView>();
            builder.Services.AddTransient<ConfigEmpresaView>();
            builder.Services.AddTransient<BonificacoesView>();
            builder.Services.AddTransient<GerenciarBonificacoesView>();
            builder.Services.AddTransient<RecompensasView>();
            builder.Services.AddTransient<GerenciarRecompensasView>();
            builder.Services.AddTransient<RelatoriosView>();
            builder.Services.AddTransient<IndicacoesView>();
            builder.Services.AddTransient<CadBonificacoesView>();
            builder.Services.AddTransient<CadRecompensaView>();
            builder.Services.AddTransient<IndicacaoConsultarEmpresaView>();
            builder.Services.AddTransient<IndicacaoEmpresaBonificacaoView>();
            builder.Services.AddTransient<IndicacaoCadastroView>();
            builder.Services.AddTransient<IndicacaoDetalhesView>();
            builder.Services.AddTransient<IndicacaoValidarQrCodeView>();
            builder.Services.AddTransient<HistIndicacoesView>();
            builder.Services.AddTransient<HistIndicacoesEmpresaView>();
            builder.Services.AddTransient<RelatoriosCoBonifView>();
            builder.Services.AddTransient<RelatoriosRankingView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}