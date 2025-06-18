
using AppMeals.Models;
using AppMeals.Services;
using AppMeals.Validations;

namespace AppMeals.Pages;

public partial class HomePage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IValidator _validator;
	private bool _loginPageDisplayed = false;


    public HomePage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        LblUserName.Text = "Hello, " + Preferences.Get("username", string.Empty);
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetCategoryList();
        await GetMostSold();
        await GetPopular();
    }

    private async Task<IEnumerable<Category>> GetCategoryList()
    {
        try
        {
            var (categories, errorMessage) = await _apiService.GetCategories();

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Category>();
            }

            if (categories == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "It was not possible to obtain categories.", "OK");
                return Enumerable.Empty<Category>();
            }

            CvCategories.ItemsSource = categories;
            return categories;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"There was an error: {ex.Message}", "OK");
            return Enumerable.Empty<Category>();
        }
    }

    private async Task<IEnumerable<Product>> GetMostSold()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("mostSold", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "It was not possible to obtain categories.", "OK");
                return Enumerable.Empty<Product>();
            }

            CVMostSold.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"There was an error: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }


    private async Task<IEnumerable<Product>> GetPopular()
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("popular", string.Empty);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "It was not possible to obtain categories.", "OK");
                return Enumerable.Empty<Product>();
            }

            CVPopulars.ItemsSource = products;
            return (IEnumerable<Product>)products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"There was an error: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

}