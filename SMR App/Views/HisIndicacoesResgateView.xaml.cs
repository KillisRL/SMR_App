using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class HisIndicacoesResgateView : ContentPage
{
    private readonly HisIndicacoesResgate _viewModel;
    public HisIndicacoesResgateView(HisIndicacoesResgate viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel = viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.IndicacaoRecompensaResgateCommand.ExecuteAsync(null);
    }

}