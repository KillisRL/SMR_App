using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class BonificacoesView : ContentPage
{
	public BonificacoesView(BonificacoesViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}