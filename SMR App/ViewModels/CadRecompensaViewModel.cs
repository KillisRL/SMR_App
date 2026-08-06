using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMRDominio.ClasseRecompensa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    public partial class CadRecompensaViewModel : BaseViewModel
    {
        private readonly ApiServiceRecompensa _apiServiceRecompensa;


        [ObservableProperty] private string? titulo;
        [ObservableProperty] private string? descricao;
        [ObservableProperty] private bool? ativo;
        [ObservableProperty] private int? pontos;

        public CadRecompensaViewModel(ApiServiceRecompensa apiServiceRecompensa)
        {
            _apiServiceRecompensa = apiServiceRecompensa;
        }

        //[RelayCommand]
        //public async Task AlterarRecompensa()
        //{
        //    try
        //    {
        //        string token = 
        //    }
        //}

        [RelayCommand]
        public async Task CadastrarRecompensa()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                if (string.IsNullOrEmpty(titulo) || string.IsNullOrEmpty(descricao) || pontos <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Atençao", "Por favor insira os dados corretamente!", "Ok");
                    return;
                }

                var pessoaLogada = ApiServicesSessaoPessoa.PessoaLogada;


                var novaRecompensa = new Recompensa
                {
                    titulo = Titulo,
                    descricao = Descricao,
                    Ativo = Ativo.Value,
                    pontos_necessarios = Pontos.Value,
                   id_empresa = pessoaLogada.id_pessoa 
                };

                var resultado = await _apiServiceRecompensa.CadastrarRecompensa(token, novaRecompensa);


                if(resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao cadastrar: {ex.Message}", "Ok");
            }
        }
    }
}
