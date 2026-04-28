using SMR_App.Services;
using SMRDominio.ClassePessoa;

namespace SMR_App.ViewModels
{
    public class PrincipalViewModel : BaseViewModel
    {
        // VARIAVEIS
        private bool _visualizarConfiguracoesEmpresa;

        // PROPRIEDADES
        public bool VisualizarConfiguracoesEmpresa
        {
            get => _visualizarConfiguracoesEmpresa;
            set
            {
                _visualizarConfiguracoesEmpresa = value;
                OnPropertyChanged();
            }
        }

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
