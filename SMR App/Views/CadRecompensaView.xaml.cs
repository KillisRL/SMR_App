using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class CadRecompensaView : ContentPage
{
    public CadRecompensaView(CadRecompensaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}