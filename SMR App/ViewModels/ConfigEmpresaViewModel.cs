using SMR_App.Services;
using SMRDominio.ClassePessoa;
using System.Windows.Input;

namespace SMR_App.ViewModels
{
    public class ConfigEmpresaViewModel : BaseViewModel
    {

        private string _nomeUsuario;
        public string NomeUsuario
        {
            get => _nomeUsuario;
            set
            {
                _nomeUsuario = value;
                OnPropertyChanged();
            }
        }

        // COMMANDS
        public ICommand AbrirTelaCommand { get; }
        public ICommand ImportarClientesCommand { get; }

        public ConfigEmpresaViewModel()
        {
            // Busca o nome do usuário logado assim que a tela for aberta
            CarregarDadosUsuario();

            // Inicialização dos Commands
            AbrirTelaCommand = new Command<string>(ExecuteAbrirTela);


        }

        private void CarregarDadosUsuario()
        {
            var pessoa = ApiServicesSessaoPessoa.PessoaLogada;

            if (pessoa != null)
            {
                NomeUsuario = pessoa.nome; 
            }
            else
            {
                NomeUsuario = "Usuário Desconhecido";
            }
        }

        // --- Métodos de Ação dos Botões ---
        private async void ExecuteAbrirTela(string nomeDaRota)
        {
            if (!string.IsNullOrWhiteSpace(nomeDaRota))
            {               
                await Shell.Current.GoToAsync(nomeDaRota);
            }
        }
    }
}
