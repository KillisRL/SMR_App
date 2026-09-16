using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class PrincipalView : ContentPage
{
	public PrincipalView(PrincipalViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}