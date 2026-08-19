using SMR_App.ViewModels;
using ZXing.Net.Maui;

namespace SMR_App.Views;

public partial class IndicacaoValidarQrCodeView : ContentPage
{
    private readonly IndicacaoValidarQrCodeViewModel _viewModel;

    public IndicacaoValidarQrCodeView(IndicacaoValidarQrCodeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        // Configura para ler apenas QR Code (melhora a velocidade)
        barcodeView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.QrCode, 
            AutoRotate = true,
            Multiple = false
        };
    }
    private async void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var primeiroResultado = e.Results?.FirstOrDefault();
        if (primeiroResultado == null) return;

        // Envia o conteúdo lido (pode ser o link ou só o código) para a ViewModel
        await _viewModel.ProcessarQrCodeLido(primeiroResultado.Value);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.IsDetecting = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.IsDetecting = false;
    }
}