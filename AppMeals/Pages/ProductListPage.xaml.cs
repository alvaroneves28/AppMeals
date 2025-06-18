
using AppMeals.Models;
using AppMeals.Services;
using AppMeals.Validations;

namespace AppMeals.Pages;

public partial class ProductListPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _categoryId;
    private bool _loginPageDisplayed = false;
    public ProductListPage(int categoryId, string categoryName , ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _categoryId = categoryId;
        Title = categoryName ?? "Products";
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductList(_categoryId);
    }

    private async Task<IEnumerable<Product>> GetProductList(int categoriaId)
    {
        try
        {
            var (products, errorMessage) = await _apiService.GetProducts("category", categoriaId.ToString());

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<Product>();
            }

            if (products is null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter as categorias.", "OK");
                return Enumerable.Empty<Product>();
            }

            CvProducts.ItemsSource = products;
            return products;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return Enumerable.Empty<Product>();
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

}
