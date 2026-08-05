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
            Routing.RegisterRoute(nameof(BonificacoesView), typeof(BonificacoesView));
            Routing.RegisterRoute(nameof(GerenciarBonificacoesView), typeof(GerenciarBonificacoesView));
            Routing.RegisterRoute(nameof(RecompensasView), typeof(RecompensasView));
            Routing.RegisterRoute(nameof(GerenciarRecompensasView), typeof(GerenciarRecompensasView));
            Routing.RegisterRoute(nameof(RelatoriosView), typeof(RelatoriosView));
            Routing.RegisterRoute(nameof(IndicacoesView), typeof(IndicacoesView));
<<<<<<< HEAD
<<<<<<< HEAD
=======
            Routing.RegisterRoute(nameof(RecuperarSenhaView), typeof(RecuperarSenhaView));
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
            Routing.RegisterRoute(nameof(RecuperarSenhaView), typeof(RecuperarSenhaView));
            Routing.RegisterRoute(nameof(CadBonificacoesView), typeof(CadBonificacoesView));
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        }
    }
}
