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
        [NotifyPropertyChangedFor(nameof(IsEdicao))]
<<<<<<< HEAD
<<<<<<< HEAD
=======
        [NotifyPropertyChangedFor(nameof(IsCadastro))]
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        private AcaoTela _acaoTela;

        [ObservableProperty] private bool _isSenhaHabilitada = true; // Por padrão aparece destravado pro Cadastro
        [ObservableProperty] private bool _isBotaoSenhaVisivel = false; // Cadeado aparece invisível

        public bool IsEdicao => AcaoTela == AcaoTela.Alteracao;
<<<<<<< HEAD
=======
        [NotifyPropertyChangedFor(nameof(IsCadastro))]
        private AcaoTela _acaoTela;

        [ObservableProperty] private bool _isSenhaHabilitada = true; // Por padrão aparece destravado pro Cadastro
        [ObservableProperty] private bool _isBotaoSenhaVisivel = false; // Cadeado aparece invisível

        public bool IsEdicao => AcaoTela == AcaoTela.Alteracao;
        public bool IsCadastro => AcaoTela == AcaoTela.Cadastro;
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
        public bool IsCadastro => AcaoTela == AcaoTela.Cadastro;
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        public string TituloPagina => AcaoTela == AcaoTela.Cadastro ? "Nova Pessoa" : "Editar Perfil";
        public string NomeBotaoAcao => AcaoTela == AcaoTela.Cadastro ? "Cadastrar" : "Salvar";

        //propriedades do clientes
        [ObservableProperty] private string nome;
<<<<<<< HEAD
<<<<<<< HEAD
        [ObservableProperty] private string nome_fantasia;
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        [ObservableProperty] private string razao_social;
        [ObservableProperty] private string documento;
        [ObservableProperty] private string celular;
        [ObservableProperty] private string telefone1;
        [ObservableProperty] private string telefone2;
        [ObservableProperty] private string email;
        [ObservableProperty] private string senha_hash;
        [ObservableProperty] private DateTime data_cadastro;
        [ObservableProperty] private int id_pessoa;
<<<<<<< HEAD
<<<<<<< HEAD
        [ObservableProperty] private bool ativo;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmpresaVisible))]
        [NotifyPropertyChangedFor(nameof(IsPromotorVisible))]
        private PessoaTipo id_pessoa_tipo;
=======
        [ObservableProperty] private bool ativo = true;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmpresaVisible))]
        [NotifyPropertyChangedFor(nameof(IsPromotorVisible))]
        private PessoaTipo id_pessoa_tipo = PessoaTipo.Promotor;
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
        [ObservableProperty] private bool ativo = true;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEmpresaVisible))]
        [NotifyPropertyChangedFor(nameof(IsPromotorVisible))]
        private PessoaTipo id_pessoa_tipo = PessoaTipo.Promotor;
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        public bool IsEmpresaVisible => Id_pessoa_tipo == PessoaTipo.Empresa;
        public bool IsPromotorVisible => Id_pessoa_tipo == PessoaTipo.Promotor;

        public ObservableCollection<PessoaTipo> pessoaTiposDisponiveis { get; }

        //[ObservableProperty] private PessoaTipo tipoUsuarioSelecionado;

        public PessoaViewModel(ApiServicesPessoa api)
        {
            _api = api;
            pessoaTiposDisponiveis = new ObservableCollection<PessoaTipo>(Enum.GetValues(typeof(PessoaTipo)).Cast<PessoaTipo>());

            // Se não veio nenhuma pessoa por parâmetro, mas a ação for carregar o perfil do utilizador logado:
            if (AcaoTela == AcaoTela.Alteracao && ApiServicesSessaoPessoa.PessoaLogada != null)
            {
                PessoaRecebida = ApiServicesSessaoPessoa.PessoaLogada;
            }
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

            int idDaEmpresa = ApiServicesSessaoPessoa.PessoaLogada.id_pessoa;

            // Salvamos na memória do celular
            Preferences.Default.Set("IdEmpresaLogada", idDaEmpresa);
        }

        [RelayCommand]
        private async Task IrParaCadastro()
        {
            await Shell.Current.GoToAsync(nameof(CadastroPessoaView));
        }

        [RelayCommand]
        private async Task Salvar()
        {
            // Validações Comuns (Todos precisam preencher)
<<<<<<< HEAD
<<<<<<< HEAD
            if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(Documento)
                || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Senha_hash))
=======
            bool senhaInvalida = (AcaoTela == AcaoTela.Cadastro && string.IsNullOrEmpty(Senha_hash));
            if (Senha_hash.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Senha precisa ter no mínimo 6 dígitos", "OK");
                return;
            }
            if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(Documento)
                || string.IsNullOrEmpty(Email) || senhaInvalida)
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
            bool senhaInvalida = (AcaoTela == AcaoTela.Cadastro && string.IsNullOrEmpty(Senha_hash));
            if (Senha_hash.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Senha precisa ter no mínimo 6 dígitos", "OK");
                return;
            }
            if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(Documento)
                || string.IsNullOrEmpty(Email) || senhaInvalida)
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor preencha os campos base.", "OK");
                return;
            }

            // Validações do Promotor
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
            // Validações da Empresa
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
                await Shell.Current.GoToAsync(nameof(LoginView));
            }
            else if (AcaoTela == AcaoTela.Alteracao)
            {
                 await AlterarPessoa();
            }
        }

        [RelayCommand]
        private async Task Excluir()
        {
            // A Regra de Ouro: Confirmação!
            bool confirmacao = await Application.Current.MainPage.DisplayAlert(
                "Atenção Cuidado!",
                "Tem certeza que deseja desativar sua conta? Você perderá o acesso ao sistema.",
                "Sim, Excluir",
                "Cancelar");

            if (!confirmacao) return; // O usuário clicou em Cancelar

<<<<<<< HEAD
<<<<<<< HEAD
            // Manda o ID para a API desativar
            var resultado = await _api.DeletarPessoaService(Id_pessoa);
=======
            string token = await SecureStorage.Default.GetAsync("jwt_token");
            // Manda o ID para a API desativar
            var resultado = await _api.DeletarPessoaService(Id_pessoa, token);
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
            string token = await SecureStorage.Default.GetAsync("jwt_token");
            // Manda o ID para a API desativar
            var resultado = await _api.DeletarPessoaService(Id_pessoa, token);
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34

            if (resultado.Sucesso)
            {
                await Application.Current.MainPage.DisplayAlert("Despedida", "Sua conta foi desativada com sucesso.", "OK");

                // Limpa a sessão local para não deixar rastro do usuário logado
                ApiServicesSessaoPessoa.EncerrarSessao();

                // Manda o usuário embora para a tela de Login
                Application.Current.MainPage = new AppShell(); // Usando "//" limpa o histórico de navegação
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Erro", resultado.Mensagem, "OK");
            }
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
=======
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        [RelayCommand]
        private void DesbloquearSenha()
        {
            IsSenhaHabilitada = true;    // Libera a digitação
            IsBotaoSenhaVisivel = false; // Esconde o cadeado
            Senha_hash = string.Empty;   // Apaga os asteriscos para ele digitar a nova senha limpa
        }

<<<<<<< HEAD
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        partial void OnPessoaRecebidaChanged(Pessoa? value)
        {
            if (value != null)
            {
                AcaoTela = AcaoTela.Alteracao;
                Id_pessoa = value.id_pessoa; 

                CarregarDadosDoBancoAsync(value.id_pessoa);
            }
        }

        private async Task CarregarDadosDoBancoAsync(int id)
        {
<<<<<<< HEAD
<<<<<<< HEAD
            var perfilCompleto = await _api.ObterPerfilCompleto(id);
=======
            string token = await SecureStorage.Default.GetAsync("jwt_token");

            var perfilCompleto = await _api.ObterPerfilCompleto(id, token);
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
            string token = await SecureStorage.Default.GetAsync("jwt_token");

            var perfilCompleto = await _api.ObterPerfilCompleto(id, token);
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34

            if (perfilCompleto != null)
            {
                // Preenchemos os campos com os dados que vieram do banco
                Nome = perfilCompleto.nome;
                Email = perfilCompleto.email;
                Id_pessoa_tipo = perfilCompleto.id_pessoa_tipo;
                Ativo = perfilCompleto.ativo ?? true;
<<<<<<< HEAD
<<<<<<< HEAD
                Senha_hash = string.Empty; // Senha sempre vazia por segurança
=======
                Senha_hash = "********";        // Visualmente preenchido
                IsSenhaHabilitada = false;      // Campo bloqueado para clique
                IsBotaoSenhaVisivel = true;     // Mostra o cadeado
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
                Senha_hash = "********";        // Visualmente preenchido
                IsSenhaHabilitada = false;      // Campo bloqueado para clique
                IsBotaoSenhaVisivel = true;     // Mostra o cadeado
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34

                if (Id_pessoa_tipo == PessoaTipo.Promotor)
                {
                    Documento = perfilCompleto.documento;
                    Celular = perfilCompleto.celular;
                }
                else if (Id_pessoa_tipo == PessoaTipo.Empresa)
                {
<<<<<<< HEAD
<<<<<<< HEAD
                    Nome = perfilCompleto.nome_fantasia;
                    Razao_social = perfilCompleto.razao_social;
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
                    Documento = perfilCompleto.documento;
                    Telefone1 = perfilCompleto.telefone1;
                    Telefone2 = perfilCompleto.telefone2;
                }
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

        private async Task AlterarPessoa()
        {
<<<<<<< HEAD
<<<<<<< HEAD
=======
            string token = await SecureStorage.Default.GetAsync("jwt_token");

>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
            string token = await SecureStorage.Default.GetAsync("jwt_token");

>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
            try
            {
                var dadosParaAlteracao = new CadastroPessoaDTO
                {
                    id_pessoa = Id_pessoa,
                    nome = Nome,
                    razao_social = Razao_social,
                    celular = Celular,
                    email = Email,
                    documento = Documento,
                    telefone1 = Telefone1,
                    telefone2 = Telefone2,
<<<<<<< HEAD
<<<<<<< HEAD
                    senha_hash = Senha_hash,
=======
                    senha_hash = Senha_hash == "********" ? string.Empty : Senha_hash,
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
                    senha_hash = Senha_hash == "********" ? string.Empty : Senha_hash,
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
                    id_pessoa_tipo = Id_pessoa_tipo,
                    ativo = Ativo,
                    data_cadastro = DateTime.Now
                };

<<<<<<< HEAD
<<<<<<< HEAD
                var resultado = await _api.AlterarPessoaService(dadosParaAlteracao);
=======
                var resultado = await _api.AlterarPessoaService(dadosParaAlteracao, token);
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
                var resultado = await _api.AlterarPessoaService(dadosParaAlteracao, token);
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34

                if (resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", "Perfil atualizado com sucesso!", "OK");

                    // Atualiza a sessão local para refletir o novo nome no cabeçalho do app imediatamente
                    if (ApiServicesSessaoPessoa.PessoaLogada != null)
                    {
                        ApiServicesSessaoPessoa.PessoaLogada.nome = Nome;
                        ApiServicesSessaoPessoa.PessoaLogada.email = Email;
                        ApiServicesSessaoPessoa.IniciarSessao(ApiServicesSessaoPessoa.PessoaLogada);
                    }

                    // Volta para a tela anterior (PrincipalView) automaticamente
                    await VoltarTelaAsync();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Falha ao salvar alterações: {ex.Message}", "OK");
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
