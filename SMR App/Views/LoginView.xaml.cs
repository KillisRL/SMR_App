using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class LoginView : ContentPage
{
    public LoginView(PessoaViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}