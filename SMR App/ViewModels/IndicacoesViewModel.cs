using SMR_App.Services;
using System.Windows.Input;

namespace SMR_App.ViewModels
{
    class IndicacoesViewModel : BaseViewModel
    {
        // Proriedades
        public string NomeUsuario { get; set; }

        private string _nomeIndicado;
        public string NomeIndicado
        {
            get => _nomeIndicado;
            set { _nomeIndicado = value; OnPropertyChanged(); }
        }

        private string _telefoneIndicado;
        public string TelefoneIndicado
        {
            get => _telefoneIndicado;
            set { _telefoneIndicado = value; OnPropertyChanged(); }
        }

        // COMANDOS
        public ICommand AvancarCommand { get; }

        public IndicacoesViewModel()
        {
            // Carrega o nome do barbeiro/promotor para o cabeçalho
            NomeUsuario = ApiServicesSessaoPessoa.PessoaLogada?.nome ?? "Usuário";

            AvancarCommand = new Command(async () => await ExecuteAvancar());
        }

        private async Task ExecuteAvancar()
        {
            if (string.IsNullOrWhiteSpace(NomeIndicado) || string.IsNullOrWhiteSpace(TelefoneIndicado))
            {
                await Shell.Current.DisplayAlert("Atenção", "Por favor, preencha o nome e o telefone.", "OK");
                return;
            }

            // Navegação para a segunda etapa passando os dados ou salvando em estado temporário
            // Exemplo enviando por parâmetro de query:
            await Shell.Current.GoToAsync($"FazerIndicacaoEtapa2View?nome={NomeIndicado}&fone={TelefoneIndicado}");
        }
    }
}
