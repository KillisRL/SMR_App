using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class CadastroPessoaView : ContentPage
{
	public CadastroPessoaView(PessoaViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}