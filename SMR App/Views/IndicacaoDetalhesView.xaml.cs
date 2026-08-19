using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class IndicacaoDetalhesView : ContentPage
{
	public IndicacaoDetalhesView(IndicacaoDetalhesViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}