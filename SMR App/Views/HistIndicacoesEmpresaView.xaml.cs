using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class HistIndicacoesEmpresaView : ContentPage
{
    private readonly HistIndicacoesEmpresaViewModel _viewModel;

    public HistIndicacoesEmpresaView(HistIndicacoesEmpresaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.IndicacaoConsultarHistoricoCommand.ExecuteAsync(null);
    }
}