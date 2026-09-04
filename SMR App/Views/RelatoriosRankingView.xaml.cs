using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class RelatoriosRankingView : ContentPage
{
	public RelatoriosRankingView(RelatoriosRankingViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}