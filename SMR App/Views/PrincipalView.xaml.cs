using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class PrincipalView : ContentPage
{
	public PrincipalView(PrincipalViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}

    // Função para fazer o menu subir
    private async void AbrirMenu_Action(object sender, EventArgs e)
    {
        // Move o menu para a posição 0 no eixo Y (traz para a tela)
        // 250 é o tempo da animação em milissegundos
        await MenuBottomSheet.TranslateTo(0, 0, 250, Easing.CubicOut);
    }

    // Função para fazer o menu descer
    private async void FecharMenu_Action(object sender, EventArgs e)
    {
        // Move o menu de volta para 500 no eixo Y (esconde para baixo)
        await MenuBottomSheet.TranslateTo(0, 500, 250, Easing.CubicIn);
    }

}