using SMR_App.Views;

namespace SMR_App
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(PessoaCadastroView), typeof(PessoaCadastroView));
            Routing.RegisterRoute(nameof(pgLoginView), typeof(pgLoginView));
        }
    }
}
