namespace AppMeals.Pages;

public partial class AddressPage : ContentPage
{
	public AddressPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSavedData();
    }

    private void LoadSavedData()
    {
        if (Preferences.ContainsKey("name"))
            EntName.Text = Preferences.Get("name", string.Empty);

        if (Preferences.ContainsKey("address"))
            EntAddress.Text = Preferences.Get("address", string.Empty);

        if (Preferences.ContainsKey("telephone"))
            EntTelephone.Text = Preferences.Get("telephone", string.Empty);
    }

    private void BtnSave_Clicked(object sender, EventArgs e)
    {
        Preferences.Set("name", EntName.Text);
        Preferences.Set("address", EntAddress.Text);
        Preferences.Set("telephone", EntTelephone.Text);
        Navigation.PopAsync();
    }
}