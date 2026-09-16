namespace SMR_App.Views;

public partial class MenuInferiorView : ContentView
{
    public MenuInferiorView()
    {
        InitializeComponent();
    }

    private async void AbrirMenu_Action(object sender, EventArgs e)
    {
        // Animação para subir o menu (mostra na tela)
        await MenuBottomSheet.TranslateTo(0, 0, 250, Easing.CubicOut);
    }

    private async void FecharMenu_Action(object sender, EventArgs e)
    {
        // Animação para descer o menu (esconde)
        await MenuBottomSheet.TranslateTo(0, 335, 250, Easing.CubicIn);
    }
}