using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Extensions;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseBase;
using SMRDominio.ClassePessoa;
using System.Collections.ObjectModel;


namespace SMR_App.ViewModels
{
    [QueryProperty(nameof(PessoaRecebida), "PessoaParaAlterar")]
    public partial class PessoaViewModel : BaseViewModel
    {

        [ObservableProperty] private Pessoa? _pessoaRecebida;
        public readonly ApiServicesPessoa _api;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TituloPagina))]
        [NotifyPropertyChangedFor(nameof(NomeBotaoAcao))]
        private AcaoTela _acaoTela;

        public string TituloPagina => AcaoTela == AcaoTela.Cadastro ? "Nova Pessoa" : "Editar Perfil";
        public string NomeBotaoAcao => AcaoTela == AcaoTela.Cadastro ? "Cadastrar" : "Salvar";

        //propriedades do clientes
        [ObservableProperty] private string nome;
        [ObservableProperty] private string documento;
        [ObservableProperty] private string telefone;
        [ObservableProperty] private string email;
        [ObservableProperty] private string senha_hash;
        [ObservableProperty] private string login;
        [ObservableProperty] private DateTime data_cadastro;
        [ObservableProperty] private PessoaTipo id_pessoatipo;
        [ObservableProperty] private int id_pessoa;

        public ObservableCollection<PessoaTipo> pessoaTiposDisponiveis { get; }

        //[ObservableProperty] private PessoaTipo tipoUsuarioSelecionado;

        public PessoaViewModel(ApiServicesPessoa api)
        {
            _api = api;
            pessoaTiposDisponiveis = new ObservableCollection<PessoaTipo>(Enum.GetValues(typeof(PessoaTipo)).Cast<PessoaTipo>());

            bool ehValido = (AcaoTela == AcaoTela.Cadastro)
                ? (Id_pessoatipo == PessoaTipo.PessoaFisica)
                : (Id_pessoatipo == PessoaTipo.PessoaJuridica);
        }

        [RelayCommand]
        private async Task Logar()
        {
            if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Senha_hash))
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha os campos.", "OK");
                return;
            }

            await LogarPessoa();
        }

        [RelayCommand]
        private async Task IrParaCadastro()
        {
            await Shell.Current.GoToAsync(nameof(CadastroPessoaView));
        }

        [RelayCommand]
        private async Task Salvar()
        {
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(documento) || string.IsNullOrEmpty(telefone) || string.IsNullOrEmpty(email)
                || string.IsNullOrEmpty(senha_hash) || string.IsNullOrEmpty(login))
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha todos os campos.", "OK");
                return;
            }
            if(Id_pessoatipo == PessoaTipo.PessoaFisica)
            {
                if (!ExtensionsValidadorCPF.CPFValido(documento))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "O CPF informado é inválido!", "OK");
                    return; // Corta a execução aqui para não salvar
                }
            }

            else if (Id_pessoatipo == PessoaTipo.PessoaJuridica)
            {
                if (!ExtensionsValidadorCNPJ.CNPJValido(documento))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "O CNPJ informado é inválido!", "OK");
                    return; // Corta a execução aqui para não salvar
                }
            }

            if (AcaoTela == AcaoTela.Cadastro)
            {
                await CadastrarPessoa();

                await Shell.Current.GoToAsync(nameof(LoginView));
            }
        }

        private async Task CadastrarPessoa()
        {
            try
            {
                var pessoaNova = new Pessoa
                {
                    nome = Nome,
                    documento = Documento,
                    telefone = Telefone,
                    email = Email,
                    login = Login,
                    senha_hash = Senha_hash,
                    id_pessoatipo = Id_pessoatipo,
                    data_cadastro = DateTime.Now,

                };
                bool sucesso = await  _api.CadastrarPessoaService(pessoaNova);
                if (sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", "Pessoa cadastrada com sucesso!", "OK");
                }
                else
                {
                    // O serviço já deve ter logado o erro, avise o usuário.
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível realizar o cadastro. Verifique os dados.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Falha de comunicação: {ex.Message}", "OK");
            }
        }

        private async Task LogarPessoa()
        {
            try
            {
                var login = new PessoaLogin
                {
                    login = Login,
                    senha_hash = Senha_hash
                };

                var pessoaRetornada = await _api.Login(login);
                if(pessoaRetornada != null)
                {
                    // Salvar dados na Pessoa Global
                    ApiServicesSessaoPessoa.IniciarSessao(pessoaRetornada);

                    // Mensagem
                    await Application.Current.MainPage.DisplayAlert($"Sucesso", $"Seja bem-vindo!", "OK");
                    // Abrir tela
                    await Shell.Current.GoToAsync("PrincipalView");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível realizar o login", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Falha de comunicação: {ex.Message}", "OK");
            }
        }

    }
}
