using SMR_App.Views;

namespace SMR_App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CadastroPessoaView), typeof(CadastroPessoaView));
            Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
            Routing.RegisterRoute(nameof(PrincipalView), typeof(PrincipalView));
            Routing.RegisterRoute(nameof(ConfigEmpresaView), typeof(ConfigEmpresaView));
            Routing.RegisterRoute(nameof(SplashView), typeof(SplashView));
        }
    }
}
