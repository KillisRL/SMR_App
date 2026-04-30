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


        public bool IsPessoaFisica => ApiServicesSessaoPessoa.PessoaLogada?.id_pessoa_tipo == PessoaTipo.PessoaFisica;

        public bool IsPessoaJuridica => ApiServicesSessaoPessoa.PessoaLogada?.id_pessoa_tipo == PessoaTipo.PessoaJuridica;

        public BaseViewModel()
        {
            CarregarDadosUsuario();
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
                await Shell.Current.GoToAsync(nomeDaRota);
            }
        }
        private void NotificarMudancaDeSessao()
        {
            // Avisa a UI para reavaliar todas as propriedades de permissão
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
            // 1. Pergunta de confirmação (boa prática de UX)
            bool confirmar = await Shell.Current.DisplayAlert("Sair",
                                                              "Deseja realmente sair do sistema?",
                                                              "Sim", "Não");
            if (!confirmar)
                return;

            // 2. Chama o serviço para limpar os dados
            ApiServicesSessaoPessoa.EncerrarSessao();

            // 3. Navegação Crítica: Usando "//" (Absolute Routing)
            // Isso é MUITO importante. Usar "//" limpa a pilha de navegação.
            // O usuário não conseguirá voltar para a tela anterior apertando "Voltar".
            //await Shell.Current.GoToAsync(nameof(pgHomeView));
        }


    }
}
