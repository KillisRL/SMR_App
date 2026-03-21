using SMR_App.ViewModels;
namespace SMR_App.Views;

public partial class PessoaCadastroView : ContentPage
{
	public PessoaCadastroView(PessoaViewModel pessoaView)
	{
		InitializeComponent();
		BindingContext = pessoaView;

    }
}