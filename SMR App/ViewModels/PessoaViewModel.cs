using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Extensions;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseBase;
using SMRDominio.ClassePessoa;
using System.Collections.ObjectModel;
using SMRDominio.DTOs;

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
        [ObservableProperty] private string razao_social;
        [ObservableProperty] private string documento;
        [ObservableProperty] private string celular;
        [ObservableProperty] private string telefone1;
        [ObservableProperty] private string telefone2;
        [ObservableProperty] private string email;
        [ObservableProperty] private string senha_hash;
        [ObservableProperty] private DateTime data_cadastro;
        [ObservableProperty] private int id_pessoa;
        [ObservableProperty] private bool ativo;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmpresaVisible))]
        [NotifyPropertyChangedFor(nameof(IsPromotorVisible))]
        private PessoaTipo id_pessoa_tipo;
        public bool IsEmpresaVisible => Id_pessoa_tipo == PessoaTipo.Empresa;
        public bool IsPromotorVisible => Id_pessoa_tipo == PessoaTipo.Promotor;

        public ObservableCollection<PessoaTipo> pessoaTiposDisponiveis { get; }

        //[ObservableProperty] private PessoaTipo tipoUsuarioSelecionado;

        public PessoaViewModel(ApiServicesPessoa api)
        {
            _api = api;
            pessoaTiposDisponiveis = new ObservableCollection<PessoaTipo>(Enum.GetValues(typeof(PessoaTipo)).Cast<PessoaTipo>());

            bool ehValido = (AcaoTela == AcaoTela.Cadastro)
                ? (Id_pessoa_tipo == PessoaTipo.Promotor)
                : (Id_pessoa_tipo == PessoaTipo.Empresa);
        }

        [RelayCommand]
        private async Task Logar()
        {
            if (string.IsNullOrEmpty(Documento) || string.IsNullOrEmpty(Senha_hash))
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
            // 1.Validações Comuns(Todo mundo tem que preencher)
            if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(Documento)
                || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Senha_hash))
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha os campos base.", "OK");
                return;
            }

            // 2. Validações do Promotor
            if (Id_pessoa_tipo == PessoaTipo.Promotor)
            {
                if (string.IsNullOrEmpty(Celular))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha o celular.", "OK");
                    return;
                }
                if (!ExtensionsValidadorCPF.CPFValido(Documento))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "O CPF informado é inválido!", "OK");
                    return;
                }
            }
            // 3. Validações da Empresa
            else if (Id_pessoa_tipo == PessoaTipo.Empresa)
            {
                if (string.IsNullOrEmpty(Razao_social))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha a Razão Social.", "OK");
                    return;
                }
                if (string.IsNullOrEmpty(Telefone1))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha o Telefone 1.", "OK");
                    return;
                }
                if (!ExtensionsValidadorCNPJ.CNPJValido(Documento))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "O CNPJ informado é inválido!", "OK");
                    return;
                }
            }

            if (AcaoTela == AcaoTela.Cadastro)
            {
                await CadastrarPessoa();
                //await Shell.Current.GoToAsync(nameof(LoginView));
            }
        }

        private async Task CadastrarPessoa()
        {
            try
            {
                var dadosParaCadastro = new CadastroPessoaDTO
                {
                    nome = Nome,
                    razao_social = Razao_social,
                    celular = Celular,
                    email = Email,
                    documento = Documento,
                    telefone1 = Telefone1,
                    telefone2 = Telefone2,
                    senha_hash = Senha_hash,
                    id_pessoa_tipo = Id_pessoa_tipo,
                    ativo = Ativo,
                    data_cadastro = DateTime.Now,
                };

                // Agora recebemos os dois retornos (Sucesso e a Mensagem)
                var resultado = await _api.CadastrarPessoaService(dadosParaCadastro);

                if (resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "OK");
                    await Shell.Current.GoToAsync(nameof(LoginView)); // Navega para o login só se der sucesso!
                }
                else
                {
                    // Exibe exatamente o erro que a API mandou (ex: Email duplicado)
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "OK");
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
                    documento = Documento,
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
