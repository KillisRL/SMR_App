using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using SMRDominio.ClasseBonificacao;
using SMR_App.Services; // Assumindo que ApiServicesSessaoPessoa está aqui

namespace SMR_App.ViewModels
{
    public class BonificacoesViewModel : BaseViewModel
    {
        // Lista observável que a interface vai "escutar"
        public ObservableCollection<Bonificacao> ListaBonificacoes { get; set; }

        public string NomeUsuario { get; set; }

        public ICommand CadastrarCommand { get; }
        public ICommand AlterarCommand { get; }

        public BonificacoesViewModel()
        {
            // Pegando o nome do usuário logado (Ajuste conforme a sua classe real)
            NomeUsuario = "Ueler Bernardo"; // Substitua por: ApiServicesSessaoPessoa.PessoaLogada.Nome;

            ListaBonificacoes = new ObservableCollection<Bonificacao>();

            CadastrarCommand = new Command(ExecuteCadastrar);
            AlterarCommand = new Command(ExecuteAlterar);

            CarregarDados();
        }

        private void CarregarDados()
        {
            // Aqui futuramente você fará o GET na sua API REST.
            // Por enquanto, dados mockados para testar o visual igual ao da imagem:
            ListaBonificacoes.Add(new Bonificacao { Id = 1, Nome = "DESCONTO DE 5 REAIS", IsMgm = true, IsAtivo = false });
            ListaBonificacoes.Add(new Bonificacao { Id = 2, Nome = "PRODUTO BRINDE", IsMgm = false, IsAtivo = true });
        }

        private async void ExecuteCadastrar()
        {
            // Lógica para ir para a tela de cadastro
            await Shell.Current.DisplayAlert("Ação", "Navegar para tela de Cadastro", "OK");
        }

        private async void ExecuteAlterar()
        {
            // Lógica para alterar (depende de como você quer selecionar o item)
            await Shell.Current.DisplayAlert("Ação", "Selecione um item para alterar", "OK");
        }
    }
}