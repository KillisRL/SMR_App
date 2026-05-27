using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMRDominio.ClassePessoa;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SMR_App.ViewModels
{
    public partial class BaseViewModel : BaseNotifyViewModel
    {
        public ICommand AbrirTelaCommand { get; }
        public ICommand VoltarTela { get; }
        public ICommand ImportarClientesCommand { get; }


        public bool IsPessoaFisica => ApiServicesSessaoPessoa.PessoaLogada?.id_pessoa_tipo == PessoaTipo.Promotor;

        public bool IsPessoaJuridica => ApiServicesSessaoPessoa.PessoaLogada?.id_pessoa_tipo == PessoaTipo.Empresa;

        public BaseViewModel()
        {
            //CarregarDadosUsuario();
            VoltarTela = new AsyncRelayCommand(VoltarTelaAsync);
            AbrirTelaCommand = new Command<string>(ExecuteAbrirTela);

            ApiServicesSessaoPessoa.OnSessaoChanged += NotificarMudancaDeSessao;
        }



        public async Task VoltarTelaAsync()
        {
            if (Application.Current?.MainPage is Shell shell)
            {
                await shell.GoToAsync("..");
            }
            else if (Application.Current?.MainPage?.Navigation.NavigationStack.Count > 1)
            {
                await Application.Current.MainPage.Navigation.PopAsync();
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


        private void CarregarDadosUsuario()
        {
            var pessoa = ApiServicesSessaoPessoa.PessoaLogada;

            if (pessoa != null)
            {
                NomeUsuario = pessoa.nome;
            }
            else
            {
                NomeUsuario = "Usuário Desconhecido";
            }
        }

        // --- Métodos de Ação dos Botões ---
        private async void ExecuteAbrirTela(string nomeDaRota)
        {
            if (!string.IsNullOrWhiteSpace(nomeDaRota))
            {
                // Se a rota for o Cadastro e tiver alguém logado, ele carrega o Perfil
                if (nomeDaRota == "CadastroPessoaView" && ApiServicesSessaoPessoa.PessoaLogada != null)
                {
                    var navigationParameter = new Dictionary<string, object>
            {
                { "PessoaParaAlterar", ApiServicesSessaoPessoa.PessoaLogada }
            };

                    await Shell.Current.GoToAsync(nomeDaRota, navigationParameter);
                }
                else
                {
                    // Para todas as outras rotas, faz a navegação normal que você já tinha
                    await Shell.Current.GoToAsync(nomeDaRota);
                }
            }
        }
        private void NotificarMudancaDeSessao()
        {
            OnPropertyChanged(nameof(IsPessoaFisica));
            OnPropertyChanged(nameof(IsPessoaJuridica));
        }

        public void Dispose()
        {
            ApiServicesSessaoPessoa.OnSessaoChanged -= NotificarMudancaDeSessao;
            // GC.SuppressFinalize(this); // Geralmente não é necessário em classes não gerenciadas
        }

        [RelayCommand]
        private async Task FazerLogoutAsync()
        {
            // Confirmação com Usuário
            bool confirmar = await Shell.Current.DisplayAlert("Sair",
                                                              "Deseja realmente sair do sistema?",
                                                              "Sim", "Não");
            if (!confirmar)
                return;

            // Chama o serviço para limpar os dados
            ApiServicesSessaoPessoa.EncerrarSessao();
            //await Shell.Current.GoToAsync(nameof(pgHomeView));
        }


    }
}
