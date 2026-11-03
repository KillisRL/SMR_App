using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMR_App.Views;
using SMRDominio.ClasseBonificacao;
using System.Collections.ObjectModel;

namespace SMR_App.ViewModels
{
    public partial class GrBonificacoesViewModel : BaseViewModel
    {
        private readonly ApiServicesBonificacao _apiServicesBonificacao;

        [ObservableProperty] private ObservableCollection<Bonificacao> listaBonificacao = new();

        public List<string> OpcoesStatus { get; } = new List<string> { "Todos", "Ativos", "Inativos" };
        [ObservableProperty] private string? statusSelecionado = "Todos";

        [ObservableProperty] private string? nome;
        [ObservableProperty] private bool? ativo;

        public GrBonificacoesViewModel(ApiServicesBonificacao apiServicesBonificacao)
        {
            _apiServicesBonificacao = apiServicesBonificacao;
            _ = ConsultarBonificacao();
        }

        partial void OnStatusSelecionadoChanged(string? value)
        {
            _ = ConsultarBonificacao();
        }

        [RelayCommand]
        public async Task ConsultarBonificacao()
        {
            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");

                // Mapeia o filtro de Status antes da chamada
                if (StatusSelecionado == "Ativos")
                    Ativo = true;
                else if (StatusSelecionado == "Inativos")
                    Ativo = false;
                else
                    Ativo = null;

                var resultado = await _apiServicesBonificacao.ConsultarBonificacao(token, Nome, Ativo);

                if (resultado.Sucesso)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ListaBonificacao.Clear();

                        if (resultado.Dados != null)
                        {
                            foreach (var item in resultado.Dados)
                            {
                                ListaBonificacao.Add(item);
                            }
                        }
                    });
                }
                else
                {
                    if (!string.IsNullOrEmpty(resultado.Mensagem) && resultado.Mensagem.Contains("Não foi encontrado"))
                    {
                        MainThread.BeginInvokeOnMainThread(() => ListaBonificacao.Clear());
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível carregar as bonificações. Erro: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task AbrirAlteracao(Bonificacao bonificacaoSelecionada)
        {
            if (bonificacaoSelecionada == null) return;

            bool confirmar = await Application.Current.MainPage.DisplayAlert("Atenção", "Deseja alterar o registro selecionado?", "Sim", "Não");

            if (!confirmar)
            {
                return;
            }

            var parametro = new Dictionary<string, object>
            {
                {"BonificacaoParaAlterar", bonificacaoSelecionada}
            };
            await Shell.Current.GoToAsync(nameof(CadBonificacoesView), parametro);
        }

        [RelayCommand]
        public async Task ExcluirBonificacao(Bonificacao bonificacao)
        {
            if (bonificacao == null) return;

            try
            {
                bool confirmacao = await Application.Current.MainPage.DisplayAlert("Atenção", "Deseja realmente excluir o registro selecionado?", "Sim", "Não");

                if (!confirmacao)
                {
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");
                int codigoBonificacao = bonificacao.Id;

                var resultado = await _apiServicesBonificacao.DeletarBonificacaoService(codigoBonificacao, token);

                if (resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                    ListaBonificacao.Remove(bonificacao); // Atualiza a UI instantaneamente
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", resultado.Mensagem, "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível excluir a bonificação. Erro: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task AbrirCadastro()
        {
            await Shell.Current.GoToAsync(nameof(CadBonificacoesView));
        }
    }
}