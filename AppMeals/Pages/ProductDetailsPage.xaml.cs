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
    public ProductDetailsPage(int productID, string productName, ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _productId = productID;
        Title = productName ?? "Product details";

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
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
            
            await DisplayAlert("Erro", errorMessage ?? "Erro in obtaining the product.", "OK");
            return null;
        }

        if (productDetail != null)
        {
            
            ProductImage.Source = productDetail.ImageUrl;
            LblProductName.Text = productDetail.Name;
            LblProductPrice.Text = productDetail.Price.ToString();
            LblProductDescription.Text = productDetail.Detail;
            LblTotalPrice.Text = productDetail.Price.ToString();
        }
        else
        {
            await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os detalhes do produto.", "OK");
            return null;
        }
        return productDetail;
    }

    private async Task DisplayLoginPage()
    {
       
        _loginPageDisplayed = true;

        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    
    }

    private void FavoriteImageBtn_Clicked(object sender, EventArgs e)
    {

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
            var shoppingCart = new ShoppingCart()
            {
                Quantity = Convert.ToInt32(LblQuantity.Text),
                Price = Convert.ToDecimal(LblProductPrice.Text),
                TotalValue = Convert.ToDecimal(LblTotalPrice.Text),
                ProductId = _productId,
                ClientId = Preferences.Get("userid", 0)
            };

            var response = await _apiService.AddItemToShoppingCart(shoppingCart);

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