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
        [ObservableProperty] private string nome_fantasia;
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
            if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(Documento)
                || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Senha_hash))
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

        partial void OnPessoaRecebidaChanged(Pessoa? value)
        {
            if (value != null)
            {
                AcaoTela = AcaoTela.Alteracao;
                Id_pessoa = value.id_pessoa; // Já guardamos o ID para a futura alteração!

                // Chamamos um método separado para buscar na API sem travar a tela
                CarregarDadosDoBancoAsync(value.id_pessoa);
            }
        }

        private async Task CarregarDadosDoBancoAsync(int id)
        {
            var perfilCompleto = await _api.ObterPerfilCompleto(id);

            if (perfilCompleto != null)
            {
                // Preenchemos os campos com os dados que vieram do banco
                Nome = perfilCompleto.nome;
                Email = perfilCompleto.email;
                Id_pessoa_tipo = perfilCompleto.id_pessoa_tipo;
                Ativo = perfilCompleto.ativo ?? true;
                Senha_hash = string.Empty; // Senha sempre vazia por segurança

                if (Id_pessoa_tipo == PessoaTipo.Promotor)
                {
                    Documento = perfilCompleto.documento;
                    Celular = perfilCompleto.celular;
                }
                else if (Id_pessoa_tipo == PessoaTipo.Empresa)
                {
                    Nome = perfilCompleto.nome_fantasia;
                    Razao_social = perfilCompleto.razao_social;
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
            try
            {
                var dadosParaAlteracao = new CadastroPessoaDTO
                {
                    id_pessoa = Id_pessoa, // Crucial para o UPDATE saber quem atualizar
                    nome = Nome,
                    razao_social = Razao_social,
                    celular = Celular,
                    email = Email,
                    documento = Documento,
                    telefone1 = Telefone1,
                    telefone2 = Telefone2,
                    senha_hash = Senha_hash, // Passa o que foi digitado (se vazio, a API ignora)
                    id_pessoa_tipo = Id_pessoa_tipo,
                    ativo = Ativo,
                    data_cadastro = DateTime.Now
                };

                var resultado = await _api.AlterarPessoaService(dadosParaAlteracao);

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
