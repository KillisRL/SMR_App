using CommunityToolkit.Mvvm.Input;
using SMR_App.Services; // Certifique-se de usar a mesma base de configuração
using System.Net.Http.Json;
using System.Windows.Input;

namespace SMR_App.ViewModels
{
    public partial class ReSenhaViewModel : BaseViewModel
    {
        private readonly HttpClient _httpClient;
        private int _faseAtual = 1;

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _codigoVerificacao;
        public string CodigoVerificacao
        {
            get => _codigoVerificacao;
            set => SetProperty(ref _codigoVerificacao, value);
        }

        private string _novaSenha;
        public string NovaSenha
        {
            get => _novaSenha;
            set => SetProperty(ref _novaSenha, value);
        }

        private string _instrucaoTexto;
        public string InstrucaoTexto
        {
            get => _instrucaoTexto;
            set => SetProperty(ref _instrucaoTexto, value);
        }

        private string _textoBotao;
        public string TextoBotao
        {
            get => _textoBotao;
            set => SetProperty(ref _textoBotao, value);
        }

        private bool _exibirCampoEmail;
        public bool ExibirCampoEmail
        {
            get => _exibirCampoEmail;
            set => SetProperty(ref _exibirCampoEmail, value);
        }

        private bool _exibirCampoCodigo;
        public bool ExibirCampoCodigo
        {
            get => _exibirCampoCodigo;
            set => SetProperty(ref _exibirCampoCodigo, value);
        }

        private bool _exibirCampoNovaSenha;
        public bool ExibirCampoNovaSenha
        {
            get => _exibirCampoNovaSenha;
            set => SetProperty(ref _exibirCampoNovaSenha, value);
        }

        public ICommand AvancarCommand { get; }
        public ICommand VoltarCommand { get; }

        public ReSenhaViewModel()
        {
            // Padronizado com o HttpClient configurado do projeto (igual às outras services)
            var handler = new HttpClientHandler { UseProxy = false };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(ConfiguracoesApp.UrlApi)
            };

            AvancarCommand = new Command(async () => await ExecutarFaseAtualAsync());
            VoltarCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            ConfigurarFase1();
        }

        private void ConfigurarFase1()
        {
            _faseAtual = 1;
            ExibirCampoEmail = true;
            ExibirCampoCodigo = false;
            ExibirCampoNovaSenha = false;
            InstrucaoTexto = "Informe o e-mail cadastrado para receber o código de recuperação.";
            TextoBotao = "ENVIAR CÓDIGO";
        }

        private async Task ExecutarFaseAtualAsync()
        {
            try
            {
                if (_faseAtual == 1)
                {
                    await SolicitarCodigoAsync();
                }
                else if (_faseAtual == 2)
                {
                    await ValidarCodigoAsync();
                }
                else if (_faseAtual == 3)
                {
                    await RedefinirSenhaAsync();
                }
            }
            catch (Exception ex)
            {
                // Mostra o erro real no Alerta para sabermos se é rota, porta ou servidor
                await Application.Current.MainPage.DisplayAlert("Erro de Conexão", ex.Message, "OK");
            }
        }

        private async Task SolicitarCodigoAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Por favor, digite seu e-mail.", "OK");
                return;
            }

            // Como o BaseAddress é a API (ex: https://localhost:7190/), chamamos direto "pessoa/solicitar-codigo"
            var response = await _httpClient.PostAsJsonAsync("pessoa/solicitar-codigo", new { Email = this.Email });

            if (response.IsSuccessStatusCode)
            {
                _faseAtual = 2;
                ExibirCampoEmail = false;
                ExibirCampoCodigo = true;
                InstrucaoTexto = "Um código de 6 dígitos foi enviado para o seu e-mail. Digite-o abaixo:";
                TextoBotao = "VALIDAR CÓDIGO";
                await Application.Current.MainPage.DisplayAlert("Sucesso!", $"Código de recuperação gerado com sucesso!", "OK");
            }
            else
            {
                var erroDetalhe = await response.Content.ReadAsStringAsync();
                await Application.Current.MainPage.DisplayAlert("Ops!", $"E-mail não encontrado ou erro no servidor. Detalhe: {erroDetalhe}", "OK");
            }
        }

        private async Task ValidarCodigoAsync()
        {
            if (string.IsNullOrWhiteSpace(CodigoVerificacao) || CodigoVerificacao.Length != 6)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "O código deve ter 6 números.", "OK");
                return;
            }

            var response = await _httpClient.PostAsJsonAsync("pessoa/validar-codigo", new
            {
                Email = this.Email,
                Codigo = this.CodigoVerificacao
            });

            if (response.IsSuccessStatusCode)
            {
                _faseAtual = 3;
                ExibirCampoCodigo = false;
                ExibirCampoNovaSenha = true;
                InstrucaoTexto = "Código validado! Agora crie a sua nova senha de acesso.";
                TextoBotao = "SALVAR NOVA SENHA";
                await Application.Current.MainPage.DisplayAlert("Sucesso!", $"Código de recuperação validado com sucesso!", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", "Código inválido ou expirado. Verifique e tente novamente.", "OK");
            }
        }

        private async Task RedefinirSenhaAsync()
        {
            if (string.IsNullOrWhiteSpace(NovaSenha) || NovaSenha.Length < 6)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "A nova senha deve ter pelo menos 6 caracteres.", "OK");
                return;
            }

            var response = await _httpClient.PostAsJsonAsync("pessoa/redefinir-senha", new
            {
                Email = this.Email,
                Codigo = this.CodigoVerificacao,
                NovaSenha = this.NovaSenha
            });

            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Sucesso", "Sua senha foi alterada com sucesso! Você já pode fazer login.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", "Erro ao redefinir a senha. Tente novamente.", "OK");
            }
        }
    }
}