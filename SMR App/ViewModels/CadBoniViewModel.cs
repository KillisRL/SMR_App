using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMRDominio.ClasseBase;
using SMRDominio.ClasseBonificacao;
using SMR_App.Services;
using System.Collections.ObjectModel;
using SMRDominio.ClasseRecompensa;

namespace SMR_App.ViewModels
{
    [QueryProperty(nameof(BonificacaoRecebida), "BonificacaoParaAlterar")]
    public partial class CadBoniViewModel : BaseViewModel
    {
        [ObservableProperty] private Bonificacao? bonificacaoRecebida; // Substitua 'object' pela sua classe/DTO real de Bonificação

        public readonly ApiServicesBonificacao _apiServicesBonificacao;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TituloPagina))]
        [NotifyPropertyChangedFor(nameof(NomeBotaoAcao))]
        [NotifyPropertyChangedFor(nameof(IsEdicao))]
        [NotifyPropertyChangedFor(nameof(IsCadastro))]
        private AcaoTela _acaoTela;

        public bool IsEdicao => AcaoTela == AcaoTela.Alteracao;
        public bool IsCadastro => AcaoTela == AcaoTela.Cadastro;
        public string TituloPagina => AcaoTela == AcaoTela.Cadastro ? "NOVA BONIFICAÇÃO" : "EDITAR BONIFICAÇÃO";
        public string NomeBotaoAcao => AcaoTela == AcaoTela.Cadastro ? "CADASTRAR" : "SALVAR";

        // ==========================================================
        // PROPRIEDADES DA TABELA / TELA
        // ==========================================================
        [ObservableProperty] private int id_bonificacao;
        [ObservableProperty] private string nome;
        [ObservableProperty] private string descricao;
        [ObservableProperty] private decimal valor;
        [ObservableProperty] private bool mgm;
        [ObservableProperty] private bool ativo = true;
        [ObservableProperty] private TipoBonificacao? tipoSelecionado;
        [ObservableProperty] private ObservableCollection<TipoBonificacao> tipos = new();

        public CadBoniViewModel(ApiServicesBonificacao api)
        {
            _apiServicesBonificacao = api;
            AcaoTela = AcaoTela.Cadastro;
            var listaTipos = Enum.GetValues(typeof(TipoBonificacao)).Cast<TipoBonificacao>();
            Tipos = new ObservableCollection<TipoBonificacao>(listaTipos);

            TipoSelecionado = Tipos.FirstOrDefault();

        }

        partial void OnBonificacaoRecebidaChanged(Bonificacao? value)
        {
            if (value != null)
            {
                AcaoTela = AcaoTela.Alteracao;

                // Preenche os campos do formulário para alteração
                Id_bonificacao = value.Id;
                Nome = value.Nome;
                Descricao = value.Descricao;
                Valor = value.Valor;
                TipoSelecionado = BonificacaoRecebida.Tipo;
                Mgm = value.Mgm;
                Ativo = value.Ativo;
            }
        }

        [RelayCommand]
        private async Task Salvar()
        {
            if (string.IsNullOrEmpty(Nome) || TipoSelecionado == null || string.IsNullOrEmpty(Descricao))
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor, preencha todos os campos", "OK");
                return;
            }

            int idEmpresaLogada = ApiServicesSessaoPessoa.PessoaLogada?.id_pessoa ?? 0;
            if (idEmpresaLogada == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Sessão Inválida", "Não foi possível identificar a empresa logada.", "OK");
                return;
            }

            if (AcaoTela == AcaoTela.Cadastro)
            {
                await CadastrarBonificacao(idEmpresaLogada);
            }
            else if (AcaoTela == AcaoTela.Alteracao)
            {
                await AlterarBonificacao(idEmpresaLogada);
            }
        }

        private async Task CadastrarBonificacao(int idEmpresa)
        {
            try
            {
                var dadosCadastro = new Bonificacao
                {
                    Id_Empresa = idEmpresa,
                    Nome = Nome,
                    Descricao = Descricao,
                    Valor = Valor,
                    Tipo = TipoSelecionado.Value,
                    Mgm = Mgm,
                    Ativo = Ativo
                };

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServicesBonificacao.CadastrarBonificacao(dadosCadastro, token);

                if (resultado.Sucesso)
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
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Falha na comunicação: {ex.Message}", "OK");
            }
        }

        private async Task AlterarBonificacao(int idEmpresa)
        {
            try
            {

                var dadosAlteracao = new Bonificacao
                {
                    Id = BonificacaoRecebida.Id,
                    Id_Empresa = idEmpresa,
                    Tipo = TipoSelecionado.Value,
                    Nome = Nome,
                    Descricao = Descricao,
                    Valor = Valor,
                    Mgm = Mgm,
                    Ativo = Ativo
                };

                string token = await SecureStorage.Default.GetAsync("jwt_token");

                var resultado = await _apiServicesBonificacao.AlterarBonificacao(dadosAlteracao, token);

                if (resultado.Sucesso)
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
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Falha ao salvar: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task Cancelar()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
