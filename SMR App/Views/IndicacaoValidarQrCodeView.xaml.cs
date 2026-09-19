using SMR_App.ViewModels;
using ZXing.Net.Maui;
using Microsoft.Maui.ApplicationModel;

namespace SMR_App.Views;

public partial class IndicacaoValidarQrCodeView : ContentPage
{
    private readonly IndicacaoValidarQrCodeViewModel _viewModel;
    private bool _processandoLeitura = false;

    public IndicacaoValidarQrCodeView(IndicacaoValidarQrCodeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        barcodeView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = false
        };
    }

    private void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_processandoLeitura) return;

        var primeiroResultado = e.Results?.FirstOrDefault();
        if (primeiroResultado == null) return;

        _processandoLeitura = true;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await _viewModel.ProcessarQrCodeLido(primeiroResultado.Value);

            _processandoLeitura = false;
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.IsDetecting = true;
        _processandoLeitura = false;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.IsDetecting = false;
    }
}