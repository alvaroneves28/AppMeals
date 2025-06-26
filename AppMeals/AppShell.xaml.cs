using AppMeals.Pages;
using AppMeals.Services;
using AppMeals.Validations;

namespace AppMeals
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
       
        public AppShell(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            ConfigureShell();
           
        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator);
            var shoppingCartPage = new ShoppingCartPage(_apiService, _validator);
            var favoritesPage = new FavoritesPage(_apiService, _validator);
            var profilePage = new ProfilePage();

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent { Title = "Home", Icon = "home", Content = homePage },
                    new ShellContent { Title = "Shopping Cart", Icon = "cart", Content = shoppingCartPage },
                    new ShellContent { Title = "Favorites", Icon = "heart", Content = favoritesPage },
                    new ShellContent { Title = "Profile", Icon = "profile", Content = profilePage }
                }
            });

        }
    }
}
