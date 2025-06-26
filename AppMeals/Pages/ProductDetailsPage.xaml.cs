using AppMeals.Models;
using AppMeals.Services;
using AppMeals.Validations;


namespace AppMeals.Pages;

public partial class ProductDetailsPage : ContentPage
{

    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _productId;
    private bool _loginPageDisplayed = false;
    private FavoritesService _favoritesService = new FavoritesService();
    public string? _imageUrl;

    public ProductDetailsPage(int productID, string? productName, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _productId = productID;
        Title = productName ?? "Product details";
        System.Diagnostics.Debug.WriteLine($"ProductDetailsPage created with ProductId={_productId}, Title={Title}");
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
        UpdateFavoriteButton();
    }

    private async Task<Product?> GetProductDetails(int productId)
    {
        var (productDetail, errorMessage) = await _apiService.GetProductDetail(productId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        if (productDetail == null)
        {
            await DisplayAlert("Erro", errorMessage ?? "Erro ao obter o produto.", "OK");
            return null;
        }

        System.Diagnostics.Debug.WriteLine($"Product loaded: {productDetail.Name}, Price: {productDetail.Price}");

        // Protege contra null nos campos
        ProductImage.Source = string.IsNullOrEmpty(productDetail.ImageUrl) ? "placeholder.png" : productDetail.ImageUrl;
        LblProductName.Text = productDetail.Name ?? "Sem nome";
        LblProductPrice.Text = productDetail.Price.ToString("F2");
        LblProductDescription.Text = productDetail.Detail ?? "Sem descrição";
        LblTotalPrice.Text = productDetail.Price.ToString("F2");
        _imageUrl = productDetail.ImageUrl;

        return productDetail;
    }

    private async Task DisplayLoginPage()
    {
       
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    
    }

    private async void FavoriteImageBtn_Clicked(object sender, EventArgs e)
    {
        try
        {
            var favoriteExists = await _favoritesService.ReadAsync(_productId);
            if (favoriteExists is not null)
            {
                await _favoritesService.DeleteAsync(favoriteExists);
            }
            else
            {
                var produtoFavorito = new FavoriteProduct()
                {
                    ProductId = _productId,
                    IsFavorite = true,
                    Detail = LblProductDescription.Text,
                    Name = LblProductName.Text,
                    Price = Convert.ToDecimal(LblProductPrice.Text),
                    ImageUrl = _imageUrl
                };

                await _favoritesService.CreateAsync(produtoFavorito);
            }
            UpdateFavoriteButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro: {ex.Message}", "OK");
        }
    }

    private async void UpdateFavoriteButton()
    {
        var favoriteExists = await
              _favoritesService.ReadAsync(_productId);

        if (favoriteExists is not null)
            FavoriteImageBtn.Source = "heartfill";
        else
            FavoriteImageBtn.Source = "heart";
    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
            decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
        
            quantity = Math.Max(1, quantity - 1);
            LblQuantity.Text = quantity.ToString();

            
            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString();
        }
        else
        {
            DisplayAlert("Erro", "Invalid Value", "OK");
        }
    }

    private void BtnIncrease_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(LblQuantity.Text, out int quantity) &&
       decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            
            quantity++;
            LblQuantity.Text = quantity.ToString();

            
            var totalPrice = quantity * unitPrice;
            LblTotalPrice.Text = totalPrice.ToString(); 
        }
        else
        {
            
            DisplayAlert("Erro", "Invalid Value", "OK");
        }
    }

    private async void BtnShoppingCartInclude_Clicked(object sender, EventArgs e)
    {
        try
        {
            var shoppingCartItem = new ShoppingCartItem()
            {
                Quantity = Convert.ToInt32(LblQuantity.Text),
                UnitPrice = Convert.ToDecimal(LblProductPrice.Text),
                TotalAmount = Convert.ToDecimal(LblTotalPrice.Text),
                ProductId = _productId,
                UserId = Preferences.Get("userid", 0)
            };

            var response = await _apiService.AddItemToShoppingCart(shoppingCartItem);

            if (response.Data)
            {
                await DisplayAlert("Success", "Item added to the cart!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", $"Failed to add item: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

}