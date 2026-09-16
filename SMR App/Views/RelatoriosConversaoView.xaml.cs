using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class RelatoriosConversaoView : ContentPage
{
	public RelatoriosConversaoView(RelatoriosConversaoViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}