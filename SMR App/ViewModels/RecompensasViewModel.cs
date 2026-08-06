using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseRecompensa;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }

        [RelayCommand]
        public async Task AbriCadastroRecompensa()
        {
            await Shell.Current.GoToAsync(nameof(CadRecompensaView));
        }


        [RelayCommand]
        public async Task ConsultarRecompensas()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServiceRecompensa.ConsultarRecompensas(token, descricao, titulo, ativo);

                if (StatusSelecionado == "Ativos")
                    Ativo = true;
                else if (StatusSelecionado == "Inativos")
                    Ativo = false;
                else
                    Ativo = null;

                if (resultado.Sucesso)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Agora isso vai funcionar perfeitamente!
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
