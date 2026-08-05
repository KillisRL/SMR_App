using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace SMR_App.ViewModels
{
    public class ReSenhaViewModel : BaseViewModel
    {
        // ==========================================================
        // 1. CONFIGURAÇÃO DA API (Ajuste para a URL do seu Swagger)
        // ==========================================================
        private readonly string _baseUrl = "https://localhost:7190/pessoa";
        private readonly HttpClient _httpClient;

        // Controle interno da máquina de estados (1 = E-mail, 2 = Código, 3 = Nova Senha)
        private int _faseAtual = 1;

        // ==========================================================
        // 2. PROPRIEDADES LIGADAS À TELA (XAML)
        // ==========================================================
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

        // ==========================================================
        // 3. COMANDOS
        // ==========================================================
        public ICommand AvancarCommand { get; }
        public ICommand VoltarCommand { get; }

        // ==========================================================
        // 4. CONSTRUTOR
        // ==========================================================
        public ReSenhaViewModel()
        {
            _httpClient = new HttpClient();

            AvancarCommand = new Command(async () => await ExecutarFaseAtualAsync());
            VoltarCommand = new Command(async () => await Shell.Current.GoToAsync(".."));

            // Prepara a tela inicial (Fase 1)
            ConfigurarFase1();
        }

        // ==========================================================
        // 5. MÉTODOS DE CONTROLE DE TELA E LÓGICA DE API
        // ==========================================================

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
                await Application.Current.MainPage.DisplayAlert("Erro", "Ocorreu um erro de conexão. Tente novamente.", "OK");
            }
        }

        private async Task SolicitarCodigoAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "Por favor, digite seu e-mail.", "OK");
                return;
            }

            // Envia para a API C#
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/solicitar-codigo", new { Email = this.Email });

            if (response.IsSuccessStatusCode)
            {
                // Sucesso, Muda a tela para a Fase 2 (Código)
                _faseAtual = 2;
                ExibirCampoEmail = false;
                ExibirCampoCodigo = true;
                InstrucaoTexto = "Um código de 6 dígitos foi enviado para o seu e-mail. Digite-o abaixo:";
                TextoBotao = "VALIDAR CÓDIGO";
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", "E-mail não encontrado no sistema.", "OK");
            }
        }

        private async Task ValidarCodigoAsync()
        {
            if (string.IsNullOrWhiteSpace(CodigoVerificacao) || CodigoVerificacao.Length != 6)
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "O código deve ter 6 números.", "OK");
                return;
            }

            // Envia para a API C#
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/validar-codigo", new
            {
                Email = this.Email,
                Codigo = this.CodigoVerificacao
            });

            if (response.IsSuccessStatusCode)
            {
                // Sucesso! Muda a tela para a Fase 3 (Nova Senha)
                _faseAtual = 3;
                ExibirCampoCodigo = false;
                ExibirCampoNovaSenha = true;
                InstrucaoTexto = "Código validado! Agora crie a sua nova senha de acesso.";
                TextoBotao = "SALVAR NOVA SENHA";
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

            // Envia para a API C#
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/redefinir-senha", new
            {
                Email = this.Email,
                Codigo = this.CodigoVerificacao,
                NovaSenha = this.NovaSenha
            });

            if (response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Sucesso", "Sua senha foi alterada com sucesso! Você já pode fazer login.", "OK");
                // Finalizou o fluxo, volta para a tela de Login
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Ops!", "Erro ao redefinir a senha. Tente novamente.", "OK");
            }
        }
    }
}