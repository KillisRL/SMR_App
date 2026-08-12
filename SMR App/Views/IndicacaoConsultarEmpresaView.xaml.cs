using SMR_App.ViewModels;

namespace SMR_App.Views
{
    public partial class IndicacaoConsultarEmpresaView : ContentPage
    {
        private readonly IndicacaoConsultarEmpresaViewModel _viewModel;

        public IndicacaoConsultarEmpresaView(IndicacaoConsultarEmpresaViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Carrega as empresas automaticamente assim que a tela abre
            if (_viewModel.ConsultarEmpresaCommand.CanExecute(null))
            {
                await _viewModel.ConsultarEmpresaCommand.ExecuteAsync(null);
            }
        }
    }
}