using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMRDominio.ClasseBase;
using SMRDominio.ClasseBonificacao;
using SMR_App.Services;
using System.Collections.ObjectModel;

namespace SMR_App.ViewModels
{
    // Recebe o objeto caso o usuário clique em "Alterar" na tela de listagem
    [QueryProperty(nameof(BonificacaoRecebida), "BonificacaoParaAlterar")]
    public partial class CadBoniViewModel : BaseViewModel
    {
        [ObservableProperty] private object? _bonificacaoRecebida; // Substitua 'object' pela sua classe/DTO real de Bonificação

        // Substitua pelo seu serviço real de API de Bonificações
        public readonly ApiServicesBonificacao _api;

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
        [ObservableProperty] private bool isMgm;
        [ObservableProperty] private bool ativo = true; // Padrão ativado no cadastro
        [ObservableProperty] private string tipoSelecionado;

        public ObservableCollection<string> TiposDisponiveis { get; }

        public CadBoniViewModel(ApiServicesBonificacao api)
        {
            _api = api;
            AcaoTela = AcaoTela.Cadastro; // Por padrão, a tela abre para cadastro

            TiposDisponiveis = new ObservableCollection<string>
            {
                "Desconto Fixo (R$)",
                "Desconto Percentual (%)",
                "Produto Brinde"
            };
        }

        // Método interceptador do Toolkit executado automaticamente quando 'BonificacaoRecebida' é preenchida pela navegação
        partial void OnBonificacaoRecebidaChanged(object? value) // Substitua 'object' pelo seu tipo real
        {
            if (value != null)
            {
                AcaoTela = AcaoTela.Alteracao;

                // Exemplo de preenchimento dos campos com o objeto recebido:
                // Id_bonificacao = value.id_bonificacao;
                // Nome = value.nome;
                // Descricao = value.descricao;
                // Valor = value.valor;
                // IsMgm = value.is_mgm;
                // Ativo = value.ativo;
                // TipoSelecionado = value.tipo;
            }
        }

        [RelayCommand]
        private async Task Salvar()
        {
            // 1. Validações Básicas
            if (string.IsNullOrEmpty(Nome) || string.IsNullOrEmpty(TipoSelecionado))
            {
                await Application.Current.MainPage.DisplayAlert("Atenção", "Por favor, preencha o Nome e o Tipo da bonificação.", "OK");
                return;
            }

            // 2. Resgata a Empresa Logada para vincular a Bonificação
            int idEmpresaLogada = ApiServicesSessaoPessoa.PessoaLogada?.id_pessoa ?? 0;
            if (idEmpresaLogada == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Sessão Inválida", "Não foi possível identificar a empresa logada.", "OK");
                return;
            }

            // 3. Direcionamento da Ação
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
                // Objeto simulando seu DTO de envio
                var dadosCadastro = new
                {
                    id_empresa = idEmpresa,
                    nome = Nome,
                    descricao = Descricao,
                    valor = Valor,
                    tipo = TipoSelecionado,
                    is_mgm = IsMgm,
                    ativo = Ativo,
                    data_cadastro = DateTime.Now
                };

                string token = await SecureStorage.Default.GetAsync("jwt_token");
                // var resultado = await _api.CadastrarBonificacaoService(dadosCadastro, token);

                // Simulando Sucesso (Adapte com o seu "resultado.Sucesso")
                await Application.Current.MainPage.DisplayAlert("Show!", "Bonificação cadastrada com sucesso.", "OK");
                await Shell.Current.GoToAsync(".."); // Volta para a tela anterior
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
                // Objeto simulando seu DTO de atualização
                var dadosAlteracao = new
                {
                    id_bonificacao = Id_bonificacao,
                    id_empresa = idEmpresa,
                    nome = Nome,
                    descricao = Descricao,
                    valor = Valor,
                    tipo = TipoSelecionado,
                    is_mgm = IsMgm,
                    ativo = Ativo
                };

                string token = await SecureStorage.Default.GetAsync("jwt_token");
                // var resultado = await _api.AlterarBonificacaoService(dadosAlteracao, token);

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Bonificação atualizada com sucesso!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro Crítico", $"Falha ao salvar: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task Excluir()
        {
            bool confirmacao = await Application.Current.MainPage.DisplayAlert(
                "Excluir Bonificação",
                $"Tem certeza que deseja excluir a bonificação '{Nome}'?",
                "Sim, Excluir",
                "Cancelar");

            if (!confirmacao) return;

            try
            {
                string token = await SecureStorage.Default.GetAsync("jwt_token");
                // var resultado = await _api.DeletarBonificacaoService(Id_bonificacao, token);

                await Application.Current.MainPage.DisplayAlert("Excluído", "Bonificação removida com sucesso.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Falha ao excluir: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task Cancelar()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
