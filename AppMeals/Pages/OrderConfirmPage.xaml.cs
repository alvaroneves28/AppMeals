namespace AppMeals.Pages;

public partial class OrderConfirmPage : ContentPage
{
	public OrderConfirmPage()
	{
		InitializeComponent();
	}

    private async void BtnRetornar_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync();
    }
}