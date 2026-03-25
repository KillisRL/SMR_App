using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class pgLoginView : ContentPage
{
	public pgLoginView(PessoaViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}