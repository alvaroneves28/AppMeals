
using AppMeals.Models;
using AppMeals.Services;
using AppMeals.Validations;

namespace AppMeals.Pages;

public partial class HomePage : ContentPage
{
	private readonly ApiService _apiService;
	private readonly IValidator _validator;
	private bool _loginPageDisplayed = false;
    private bool _isDataLoaded = false;
    


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
        if (!_isDataLoaded){
            await LoadDataAsync();
            _isDataLoaded = true;
        }
    //    await GetCategoryList();
    //    await GetMostSold();
    //    await GetPopular();
    }

    private async Task LoadDataAsync()
    {
        var categoriesTask = GetCategoryList();
        var mostSoldTask = GetMostSold();
        var popularsTask = GetPopular();

        await Task.WhenAll(categoriesTask, mostSoldTask, popularsTask);
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

    private async void CvCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
            return;

        var currentSelection = e.CurrentSelection.FirstOrDefault() as Category;
        if (currentSelection is null || string.IsNullOrWhiteSpace(currentSelection.Name))
            return;

        ((CollectionView)sender).SelectedItem = null;

        await Navigation.PushAsync(new ProductListPage(currentSelection.Id,
                                                        currentSelection.Name,
                                                        _apiService,
                                                        _validator));
    }

    private async void CVMostSold_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            await NavigateToProductDetailsPage(collectionView, e);
        }
    }

    private async void CVPopulars_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is CollectionView collectionView)
        {
            await NavigateToProductDetailsPage(collectionView, e);
        }
    }

    private async Task NavigateToProductDetailsPage(CollectionView collectionView, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as Product;

        if (currentSelection == null)
            return;

        await Navigation.PushAsync(new ProductDetailsPage(
                                 currentSelection.Id, currentSelection.Name!, _apiService, _validator
        ));

        collectionView.SelectedItem = null;
    }

}