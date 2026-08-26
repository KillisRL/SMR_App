using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseRecompensa;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    public partial class RecompensasViewModel : BaseViewModel
    {
        private readonly ApiServiceRecompensa _apiServiceRecompensa;

        [ObservableProperty] ObservableCollection<Recompensa> listaRecompensa = new();
        [ObservableProperty] private bool? ativo;
        [ObservableProperty] private string? titulo;
        [ObservableProperty] private string? descricao;

        public List<string> OpcoesStatus { get; } = new List<string> { "Todos", "Ativos", "Inativos" };
        [ObservableProperty] private string statusSelecionado = "Todos";

        public RecompensasViewModel(ApiServiceRecompensa apiServiceRecompensa) 
        {
            _apiServiceRecompensa = apiServiceRecompensa;
            _ = ConsultarRecompensas();
        }

        [RelayCommand]
        public async Task AbriCadastroRecompensa()
        {
            await Shell.Current.GoToAsync(nameof(CadRecompensaView));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool ativo && ativo)
                return "ATIVO";
            return "INATIVO";
        }

        [RelayCommand]
        public async Task ExcluirRecompensa(Recompensa recompensa)
        {
            try
            {
                bool confimacao = await Application.Current.MainPage.DisplayAlert("Atenção", "Deseja realmente excluir a recompensa selecionada", "Sim", "Não");

                if(!confimacao)
                {
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token"); 

                int codigoRecompensa = recompensa.id;

                var resultado = await _apiServiceRecompensa.ExcluirRecompensa(token, codigoRecompensa);

                if(resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                    return;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", resultado.Mensagem, "Ok");
                    return;
                }
            }

            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível excluir a recompensa selecionada. Erro: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task AbrirAlteracao(Recompensa recompensa)
        {
            bool confimacao = await Application.Current.MainPage.DisplayAlert("Atenção", "Deseja realizar alteração da Recompensa", "Sim", "Não");
            if(!confimacao)
            {
                return;
            }
            var parametro = new Dictionary<string, object>
            {
                {"RecompensaSelecionada", recompensa }
            };
            await Shell.Current.GoToAsync(nameof(CadRecompensaView),parametro);
        }

        partial void OnStatusSelecionadoChanged(string value)
        {
            _ = ConsultarRecompensas();
        }

        [RelayCommand]
        public async Task ConsultarRecompensas()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                if (StatusSelecionado == "Ativos")
                    Ativo = true;
                else if (StatusSelecionado == "Inativos")
                    Ativo = false;
                else
                    Ativo = null;

                var resultado = await _apiServiceRecompensa.ConsultarRecompensas(token, Descricao, Titulo, Ativo);

                if (resultado.Sucesso)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaRecompensa.Clear();

                        if (resultado.Dados != null)
                        {
                            foreach (var item in resultado.Dados)
                            {
                                ListaRecompensa.Add(item);
                            }
                        }
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(resultado.Mensagem) && resultado.Mensagem.Contains("Não foi encontrado"))
                    {
                        MainThread.BeginInvokeOnMainThread(() => ListaRecompensa.Clear());
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível carregar as recompensas. Erro: {ex.Message}", "OK");
            }
        }
    }
}
