using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class GerenciarBonificacoesView : ContentPage
{
	private readonly GrBonificacoesViewModel _viewModel;
	public GerenciarBonificacoesView(GrBonificacoesViewModel viewModel)
	{
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.ConsultarBonificacao();
    }
}