using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class IndicacaoEmpresaBonificacaoView : ContentPage
{
	public IndicacaoEmpresaBonificacaoView(IndicacaoEmpresaBonificacaoViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}