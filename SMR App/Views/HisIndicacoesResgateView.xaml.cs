using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class HisIndicacoesResgateView : ContentPage
{
    private readonly HisIndicacoesResgateViewModel _viewModel;
    public HisIndicacoesResgateView(HisIndicacoesResgateViewModel viewModel)
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