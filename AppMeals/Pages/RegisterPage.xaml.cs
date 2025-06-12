using AppMeals.Services;
using System.Threading.Tasks;

namespace AppMeals.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    public RegisterPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;
    }

    private async void btnSignup_Clicked(object sender, EventArgs e)
    {
        var response = await _apiService.RegisterUser(EntName.Text, EntEmail.Text, EntContact.Text, EntPassword.Text);

        if (response.HasError)
        {
            await DisplayAlert("Warning", "Your account was created successfully!", "ok");
            await Navigation.PushAsync(new LoginPage(_apiService));
        }
        else
        {
            await DisplayAlert("Erro", "Something went wrong!!!", "Cancel");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage(_apiService));
    }
}