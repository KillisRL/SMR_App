using CommunityToolkit.Mvvm.ComponentModel;
using SMR_App.Services;
using SMRDominio.ClassePessoa;

namespace SMR_App.ViewModels
{
    public class PrincipalViewModel : BaseViewModel
    {

        // VARIÁVEIS DE PERFIL
        private bool _isEmpresa;
        private bool _isCliente;

        // PROPRIEDADES
        public bool IsEmpresa
        {
            get => _isEmpresa;
            set
            {
                _isEmpresa = value;
                OnPropertyChanged();
            }
        }

        public bool IsCliente
        {
            get => _isCliente;
            set
            {
                _isCliente = value;
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

            if (pessoa != null)
            {
                // Define os perfis com base no tipo da pessoa logada
                IsEmpresa = (pessoa.id_pessoa_tipo == PessoaTipo.Empresa);

                // Se não for empresa, assumimos que é o cliente (pessoa física)
                // Se você tiver um Enum específico para cliente, pode usar: pessoa.id_pessoa_tipo == PessoaTipo.Cliente
                IsCliente = (pessoa.id_pessoa_tipo != PessoaTipo.Empresa);

                NomeUsuario = pessoa.nome;
            }
            else
            {
                IsEmpresa = false;
                IsCliente = false;
            }
        }
    }
}
