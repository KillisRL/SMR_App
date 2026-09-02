using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class RelatoriosView : ContentPage
{
	public RelatoriosView(RelatoriosViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}