using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class GerenciarRecompensasView : ContentPage
{
    private readonly GrRecompensasViewModel _viewModel;
    public GerenciarRecompensasView(GrRecompensasViewModel viewModel)
	{
		 
		InitializeComponent();

		_viewModel = viewModel;
		BindingContext = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.ConsultarHistorico();
    }
}