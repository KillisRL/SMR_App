using CommunityToolkit.Mvvm.ComponentModel;
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
                VisualizarConfiguracoesEmpresa = (pessoa.id_pessoa_tipo == PessoaTipo.Empresa);

                NomeUsuario = pessoa.nome;
            }
            else
            {
                VisualizarConfiguracoesEmpresa = false;
            }
        }
    }
}
