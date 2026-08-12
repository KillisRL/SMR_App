using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class BonificacoesView : ContentPage
{
    private readonly BonificacoesViewModel _viewModel;

    public BonificacoesView(BonificacoesViewModel viewModel)
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