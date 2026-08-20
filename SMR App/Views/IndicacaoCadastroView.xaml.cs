using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class IndicacaoCadastroView : ContentPage
{
	public IndicacaoCadastroView(IndicacaoCadastroViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}