using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class RecompensasView : ContentPage
{
    // O .NET MAUI vai injetar a ViewModel automaticamente aqui
    public RecompensasView(RecompensasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Seleciona Título por padrão no filtro
        if (PickerTipoFiltro != null)
            PickerTipoFiltro.SelectedIndex = 0;
    }

    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is RecompensasViewModel vm)
        {
            if (PickerTipoFiltro.SelectedIndex == 0) // Título
            {
                vm.Titulo = e.NewTextValue;
                vm.Descricao = null;
            }
            else // Descrição
            {
                vm.Descricao = e.NewTextValue;
                vm.Titulo = null;
            }
        }
    }

    private void OnTipoFiltroChanged(object sender, EventArgs e)
    {
        if (BindingContext is RecompensasViewModel vm)
        {
            SearchBarPesquisa.Text = string.Empty;
            vm.Titulo = null;
            vm.Descricao = null;
        }
    }
}