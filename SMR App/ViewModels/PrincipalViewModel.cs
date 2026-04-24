using SMR_App.Services;
using SMRDominio.ClassePessoa;

namespace SMR_App.ViewModels
{
    public class PrincipalViewModel : BaseViewModel
    {
        // Variável privada
        private bool _visualizarConfiguracoesEmpresa;

        // Propriedade pública que a View (XAML) vai enxergar
        public bool VisualizarConfiguracoesEmpresa
        {
            get => _visualizarConfiguracoesEmpresa;
            set
            {
                _visualizarConfiguracoesEmpresa = value;
                OnPropertyChanged(); // Avisa a tela que o valor mudou!
            }
        }

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

        public PrincipalViewModel()
        {
            ValidarPermissoesDeMenu();
        }

        private void ValidarPermissoesDeMenu()
        {
            // Obter pessoa logada
            var pessoa = ApiServicesSessaoPessoa.PessoaLogada;

            // Se for pessoa jurídica aparece o menu de configuração de empresa
            if (pessoa != null)
            {
                VisualizarConfiguracoesEmpresa = (pessoa.id_pessoatipo == PessoaTipo.PessoaJuridica);

                NomeUsuario = pessoa.nome;
            }
            else
            {
                VisualizarConfiguracoesEmpresa = false;
            }
        }
    }
}
