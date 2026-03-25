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

            //Views
            builder.Services.AddTransient<PessoaViewModel>();
            builder.Services.AddTransient<pgLoginView>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
