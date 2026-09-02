using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class RelatoriosCoBonifView : ContentPage
{
	public RelatoriosCoBonifView(RelatoriosCoBonifViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}