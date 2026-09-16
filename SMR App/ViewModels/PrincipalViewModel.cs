using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMRDominio.ClassePessoa;

namespace SMR_App.ViewModels
{
    public partial class PrincipalViewModel : BaseViewModel
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

        [RelayCommand]
        private async Task Logout()
        {
            bool confirmar = await Application.Current.MainPage.DisplayAlert("Sair", "Tem certeza que deseja sair da sua conta?", "Sim", "Cancelar");

            if (confirmar)
            {
                // 1. Limpa a classe estática de Sessão
                ApiServicesSessaoPessoa.EncerrarSessao();

                // 2. Remove os dados salvos fisicamente no aparelho
                SecureStorage.Default.Remove("jwt_token");
                Preferences.Default.Remove("IdEmpresaLogada");

                // 3. Usa a rota ABSOLUTA "//" para destruir o histórico de navegação
                // Isso impede que o usuário aperte o botão "Voltar" do celular e caia na tela logada
                await Shell.Current.GoToAsync("///LoginView");
            }
        }
    }
}
