using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMR_App.Services;
using SMRDominio.ClassePessoa;
using SMRDominio.ClasseRecompensa;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    [QueryProperty(nameof(RecompensaParaEditar), "RecompensaSelecionada")]
    public partial class CadRecompensaViewModel : BaseViewModel
    {
        private readonly ApiServiceRecompensa _apiServiceRecompensa;

        [ObservableProperty] private string? titulo;
        [ObservableProperty] private string? descricao;
        [ObservableProperty] private bool ativo = true; // Por padrão inicia como ativa
        [ObservableProperty] private int? pontos;
        [ObservableProperty] private Recompensa_Rank? rankSelecionado;
        [ObservableProperty] private ObservableCollection<Recompensa_Rank> ranks = new();

        [ObservableProperty] private Recompensa? recompensaParaEditar;

        public CadRecompensaViewModel(ApiServiceRecompensa apiServiceRecompensa)
        {
            _apiServiceRecompensa = apiServiceRecompensa;

            
            var listaRanks = Enum.GetValues(typeof(Recompensa_Rank)).Cast<Recompensa_Rank>();
            Ranks = new ObservableCollection<Recompensa_Rank>(listaRanks);

            if (recompensaParaEditar != null)
            {
                RankSelecionado = RecompensaParaEditar.id_rank.Value;
            }
            else
            { 
                RankSelecionado = Ranks.FirstOrDefault();
            }
        }


        partial void OnRecompensaParaEditarChanged(Recompensa? value)
        {
            if (value != null)
            {
                // Preenche os campos da tela com os dados da recompensa a ser alterada
                Titulo = value.titulo;
                Descricao = value.descricao;
                Pontos = value.pontos_necessarios;
                Ativo = value.Ativo;
            }
        }


        [RelayCommand]
        public async Task CadastrarOuAlterarRecompensa()
        {

            if(RecompensaParaEditar != null)
            {
                AlterarRecompensa();
            }
            else
            {
                CadastrarRecompensa();
            }
        }

        [RelayCommand]
        public async Task AlterarRecompensa()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Titulo) ||
                 string.IsNullOrWhiteSpace(Descricao) ||
                 !Pontos.HasValue || Pontos.Value <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor, preencha todos os campos obrigatórios corretamente!", "Ok");
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var pessoaLogada = ApiServicesSessaoPessoa.PessoaLogada;

                var recompensaAlterar = new Recompensa
                {
                    id = RecompensaParaEditar.id,
                    titulo = Titulo,
                    descricao = Descricao,
                    id_rank = RankSelecionado,
                    pontos_necessarios = Pontos.Value,
                    id_empresa = pessoaLogada.id_pessoa,
                    Ativo = Ativo
                };

                var resultado = await _apiServiceRecompensa.AlterarRecompensa(token, recompensaAlterar);

                if(resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", resultado.Mensagem, "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao cadastrar: {ex.Message}", "Ok");
            }
        }

        [RelayCommand]
        public async Task CadastrarRecompensa()
        {
            try
            {
                // 1. Validação básica de campos obrigatórios
                if (string.IsNullOrWhiteSpace(Titulo) ||
                    string.IsNullOrWhiteSpace(Descricao) ||
                    !Pontos.HasValue || Pontos.Value <= 0 )
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor, preencha todos os campos obrigatórios corretamente!", "Ok");
                    return;
                }

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                if (string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", "Sessão expirada. Faça login novamente.", "Ok");
                    return;
                }

                var pessoaLogada = ApiServicesSessaoPessoa.PessoaLogada;

                // 2. Montagem do objeto Recompensa
                var novaRecompensa = new Recompensa
                {
                    titulo = Titulo,
                    descricao = Descricao,
                    Ativo = Ativo,
                    pontos_necessarios = Pontos.Value,
                    id_empresa = pessoaLogada?.id_pessoa ?? 0,
                    // Se a FK id_rank no banco for int, enviamos o valor do enum convertido:
                    id_rank = RankSelecionado.Value
                };

                // 3. Chamada do serviço da API
                var resultado = await _apiServiceRecompensa.CadastrarRecompensa(token, novaRecompensa);

                if (resultado.Sucesso)
                {
                    await Application.Current.MainPage.DisplayAlert("Sucesso", resultado.Mensagem, "Ok");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Atenção", resultado.Mensagem, "Ok");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao cadastrar: {ex.Message}", "Ok");
            }
        }
    }
}