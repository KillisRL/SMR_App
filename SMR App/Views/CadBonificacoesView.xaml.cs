using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class CadBonificacoesView : ContentPage
{
	public CadBonificacoesView(CadBoniViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}