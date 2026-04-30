using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class RecompensasView : ContentPage
{
	public RecompensasView(RecompensasViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}