namespace SMR_App.Views;

public partial class SplashView : ContentPage
{
	public SplashView()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(2000);

        await Shell.Current.GoToAsync($"{nameof(LoginView)}");
    }
}