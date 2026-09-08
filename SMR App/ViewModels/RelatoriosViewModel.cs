using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SMR_App.ViewModels
{
    public partial class RelatoriosViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _nomeUsuario = "Ueler Bernardo";

        [ObservableProperty]
        private string _textoPesquisa = string.Empty;

        [RelayCommand]
        private async Task VoltarInicio()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        [RelayCommand]
        private async Task IrParaControleBonificacao()
        {
            // Navega para a tela de controle de bonificação com gráfico
            await Shell.Current.GoToAsync(nameof(Views.RelatoriosCoBonifView));
        }

        [RelayCommand]
        private async Task IrParaRankingPromotores()
        {
            // Futura tela de ranking
        }

        [RelayCommand]
        private async Task IrParaConversao()
        {
            // Futura tela de conversão
        }
    }
}