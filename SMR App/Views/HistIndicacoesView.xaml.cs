using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class HistIndicacoesView : ContentPage
{
	public HistIndicacoesView(HistIndicacoesViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}