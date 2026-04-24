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
                OnPropertyChanged(); // Avisa a tela para atualizar o texto
            }
        }

        // Commands para os botões da tela
        public ICommand AbrirRecompensasCommand { get; }
        public ICommand GerenciarRecompensasCommand { get; }
        public ICommand AbrirBonificacoesCommand { get; }
        public ICommand GerenciarBonificacoesCommand { get; }
        public ICommand AbrirRelatoriosCommand { get; }
        public ICommand ImportarClientesCommand { get; }

        public ConfigEmpresaViewModel()
        {
            // Busca o nome do usuário logado assim que a tela for aberta
            CarregarDadosUsuario();

            // Inicialização dos Commands (Exemplo de como preparar para os cliques)
            AbrirRecompensasCommand = new Command(ExecuteAbrirRecompensas);
            // ... (inicialize os outros commands aqui)
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
        private void ExecuteAbrirRecompensas()
        {
            // Lógica para ir para a tela de recompensas (ex: Shell.Current.GoToAsync(...))
        }
    }
}
