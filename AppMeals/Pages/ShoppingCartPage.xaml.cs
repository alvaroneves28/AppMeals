using AppMeals.Models;
using AppMeals.Services;
using AppMeals.Validations;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AppMeals.Pages;

public partial class ShoppingCartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;
    private bool _isNavigatingToEmptyCartPage = false;

    private readonly ObservableCollection<ShoppingCartItem> ShoppingCartItems = new();

    public ShoppingCartPage(ApiService apiService, IValidator validator)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    private async Task<bool> GetShoppingCartItems()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            
            var (shoppingCartItems, errorMessage) = await _apiService.GetShoppingCartItems(userId);

            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return false;
            }

            if (shoppingCartItems == null)
            {
                await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os itens do carrinho de compra.", "OK");
                return false;
            }

            ShoppingCartItems.Clear();
            foreach (var item in shoppingCartItems)
            {
                ShoppingCartItems.Add(item);
                item.UserId = userId;
            }

            CvShoppingCart.ItemsSource = ShoppingCartItems;
            UpdateTotalPrice();

            return ShoppingCartItems.Any();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            return false;
        }
    }

    private void UpdateTotalPrice()
    {
        try
        {
            var totalPrice = ShoppingCartItems.Sum(item => item.UnitPrice * item.Quantity);
            LblTotalPrice.Text = totalPrice.ToString("F2"); 
        }
        catch (Exception ex)
        {
            DisplayAlert("Erro", $"Ocorreu um erro ao atualizar o preço total: {ex.Message}", "OK");
        }
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void BtnDecrease_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem shoppingCartItem)
        {
            if (shoppingCartItem.Quantity == 1) return;
            else
            {
                shoppingCartItem.Quantity--;
                UpdateTotalPrice();
                await _apiService.UpdateShoppingCartItemQuantity(shoppingCartItem.ProductId, "decreased");
            }
                
        }
    }

    private async void BtnIncrease_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem shoppingCartItem)
        {
            shoppingCartItem.Quantity++;
            UpdateTotalPrice();
            await _apiService.UpdateShoppingCartItemQuantity(shoppingCartItem.ProductId, "Increased");
        }
    }

    private void BtnEditAdress_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddressPage());
    }

    private void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetShoppingCartItems();

        bool savedAddress = Preferences.ContainsKey("address");

        if (savedAddress)
        {
            string name = Preferences.Get("name", string.Empty);
            string address = Preferences.Get("address", string.Empty);
            string telephone = Preferences.Get("telephone", string.Empty);

            LblAddress.Text = $"{name}\n{address}\n{telephone}";
        }
        else
        {
            LblAddress.Text = "Introduce your address";
        }
    }

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ShoppingCartItem shoppingCartItem)
        {
            bool response = await DisplayAlert("Confirmation", "Are you sure you want to delete this sitem from the shopping cart ?", "yes", "no");
            if (response)
            {
                ShoppingCartItems.Remove(shoppingCartItem);
                UpdateTotalPrice();
                await _apiService.UpdateShoppingCartItemQuantity(shoppingCartItem.ProductId, "delete");
            }

        }
    }
}
