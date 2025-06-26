using AppMeals.Services;
using AppMeals.Validations;
using System.Threading.Tasks;

namespace AppMeals.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    
    public RegisterPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
     
    }

    private async void btnSignup_Clicked(object sender, EventArgs e)
    {
        if (await _validator.Validate(EntName.Text, EntEmail.Text, EntContact.Text, EntPassword.Text))
        {
            var response = await _apiService.RegisterUser(EntName.Text, EntEmail.Text, EntContact.Text, EntPassword.Text);

            if (response.Success)
            {
                await DisplayAlert("Notice", "Your account has been successfully created!!", "OK");
                await Navigation.PushAsync(new LoginPage(_apiService, _validator));
            }
            else
            {
                await DisplayAlert("Error", "Something went wrong!!!", "Cancel");
            }
        }
        else
        {
            string errorMessage = "";
            errorMessage += _validator.NameError != null ? $"\n - {_validator.NameError}" : "";
            errorMessage += _validator.EmailError != null ? $"\n - {_validator.EmailError}" : "";
            errorMessage += _validator.ContactError != null ? $"\n - {_validator.ContactError}" : "";
            errorMessage += _validator.PasswordError != null ? $"\n - {_validator.PasswordError}" : "";

            await DisplayAlert("Error", errorMessage, "OK");
        }
    }
        private async void TapLogin_Tapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new LoginPage(_apiService, _validator));
        }
    
}