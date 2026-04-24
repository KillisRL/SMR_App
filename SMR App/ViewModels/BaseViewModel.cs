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
        public ICommand AbrirTela { get; }
        public ICommand VoltarTela { get; }


        public bool IsPessoaFisica => ApiServicesSessaoPessoa.PessoaLogada?.id_pessoatipo == PessoaTipo.PessoaFisica;

        public bool IsPessoaJuridica => ApiServicesSessaoPessoa.PessoaLogada?.id_pessoatipo == PessoaTipo.PessoaJuridica;

        public BaseViewModel()
        {
            VoltarTela = new AsyncRelayCommand(VoltarTelaAsync);
            AbrirTela = new AsyncRelayCommand<Type>(AbrirTelaAsync);

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

        public async Task AbrirTelaAsync(Type pageType)
        {
            if (pageType == null)
            {
                Debug.WriteLine("Erro: Tipo de página para navegação é nulo.");
                return;
            }

            try
            {
                // Verifica se a página atual já é a página de destino (para evitar empilhar a mesma página)
                // Isso pode ser útil para rotas simples, mas com o Shell você pode usar rotas absolutas
                // ou verificar o stack do Shell.

                // Melhor usar o Shell para navegação, se você estiver usando Shell
                if (Application.Current?.MainPage is Shell shell)
                {
                    // Usa o nome da rota registrado no AppShell
                    await shell.GoToAsync(pageType.Name);
                }
                else
                {
                    // Fallback para navegação tradicional se não for Shell ou em outro contexto
                    var page = App.Current.Handler.MauiContext.Services.GetService(pageType) as ContentPage;
                    if (page != null)
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(page);
                    }
                    else
                    {
                        Debug.WriteLine($"Erro: Não foi possível resolver a página do tipo {pageType.Name}. Verifique o registro no MauiProgram.cs.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao navegar para {pageType.Name}: {ex.Message}");
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
