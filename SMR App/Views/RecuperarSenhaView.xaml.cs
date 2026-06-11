using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class RecuperarSenhaView : ContentPage
{
	public RecuperarSenhaView(ReSenhaViewModel viewModel)
	{
		InitializeComponent();

        BindingContext = viewModel;
    }
}