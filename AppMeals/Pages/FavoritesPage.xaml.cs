
using AppMeals.Models;
using AppMeals.Services;
using AppMeals.Validations;

namespace AppMeals.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoritesService _favoritesService = new FavoritesService();
    public FavoritesPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetFavoriteProducts();
    }

    private async Task GetFavoriteProducts()
    {
        try
        {
            var produtosFavoritos = await _favoritesService.ReadAllAsync();

            if (produtosFavoritos is null || produtosFavoritos.Count == 0)
            {
                CvProducts.ItemsSource = null;//limpa a lista atual
                LblWarning.IsVisible = true; //mostra o aviso
            }
            else
            {
                CvProducts.ItemsSource = produtosFavoritos;
                LblWarning.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"There was an unexpected error: {ex.Message}", "OK");
        }
    }

    private async void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteProduct;

        if (currentSelection == null)
        {
            await DisplayAlert("Aviso", "Nenhum produto foi selecionado corretamente.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(currentSelection.Name))
        {
            await DisplayAlert("Erro", "O produto não tem nome definido.", "OK");
            return;
        }

        try
        {
            await Navigation.PushAsync(new ProductDetailsPage(
                currentSelection.ProductId,
                currentSelection.Name,
                _apiService,
                _validator
                ));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao abrir a página de detalhes: {ex.Message}", "OK");
        }

    ((CollectionView)sender).SelectedItem = null;
    }

}