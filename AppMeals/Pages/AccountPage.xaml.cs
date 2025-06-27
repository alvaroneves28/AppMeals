using AppMeals.Services;
using AppMeals.Validations;

namespace AppMeals.Pages;

public partial class AccountPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private const string UserNameKey = "username";
    private const string UserEmailKey = "email";
    private const string UserPhoneKey = "contact";


    public AccountPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        GetUserInfo();
        ImgBtnProfile.Source = await GetProfileImageAsync();
    }

    private async Task<ImageSource> GetProfileImageAsync()
    {
        string imagemPadrao = AppConfig.DefaultProfileImage;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Erro", "N o autorizado", "OK");
                    return imagemPadrao;
                default:
                    await DisplayAlert("Erro", errorMessage ?? "N o foi poss vel obter a imagem.", "OK");
                    return imagemPadrao;
            }
        }

        if (response?.ImageUrl is not null)
        {
            return response.ImagePath;
        }
        return imagemPadrao;
    }

    private void GetUserInfo()
    {
        LblUserName.Text = Preferences.Get(UserNameKey, string.Empty);
        EntName.Text = LblUserName.Text;
        EntEmail.Text = Preferences.Get(UserEmailKey, string.Empty);
        EntPhone.Text = Preferences.Get(UserPhoneKey, string.Empty);
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(UserNameKey, EntName.Text);
        Preferences.Set(UserEmailKey, EntEmail.Text);
        Preferences.Set(UserPhoneKey, EntPhone.Text);
        await DisplayAlert("Data Saved", "Your information was successfully stored.", "OK");

    }
    
    
}