using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SMR_App.Services;
using SMR_App.ViewModels;
using SMR_App.Views;

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //Serviços
            //builder.Services.AddSingleton<ApiServicesSessaoPessoa>();
            builder.Services.AddSingleton<ApiServicesPessoa>();

            //ViewModels
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

            //Views
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
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
