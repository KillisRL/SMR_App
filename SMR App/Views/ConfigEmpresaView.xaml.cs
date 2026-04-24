using SMR_App.ViewModels;

namespace SMR_App.Views;

public partial class ConfigEmpresaView : ContentPage
{
	public ConfigEmpresaView(ConfigEmpresaViewModel viewlModel)
	{
		InitializeComponent();

		BindingContext = viewlModel;
	}
}