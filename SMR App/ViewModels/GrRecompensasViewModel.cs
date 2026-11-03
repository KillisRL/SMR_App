using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMRDominio.ClasseRecompensa;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    public partial class GrRecompensasViewModel : BaseViewModel
    {
        private readonly ApiServiceRecompensa _apiServiceRecompensa;

        [ObservableProperty] private string buscaNome = string.Empty;

        [ObservableProperty] private ObservableCollection<RecompensaDistribuidaDto> listaHistorico = new();

        public GrRecompensasViewModel(ApiServiceRecompensa apiServiceRecompensa)
        {
            _apiServiceRecompensa = apiServiceRecompensa;
            _ = ConsultarHistorico();
        }

        [RelayCommand]
        public async Task ConsultarHistorico()
        {
            try
            {
                string? token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceRecompensa.ConsultarHistoricoEntregues(token, BuscaNome);

                if (resultado.Sucesso)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaHistorico.Clear();
                        if (resultado.Dados != null)
                        {
                            foreach (var item in resultado.Dados)
                            {
                                ListaHistorico.Add(item);
                            }
                        }
                    });
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível carregar o histórico.", "OK");
            }
        }
    }
}