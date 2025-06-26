using AppMeals.Pages;
using AppMeals.Services;
using AppMeals.Validations;

namespace AppMeals
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly FavoritesService _favoritesService;
        public App(ApiService apiService, IValidator validator, FavoritesService favoritesService)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
            _favoritesService = favoritesService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var accessToken = Preferences.Get("accesstoken", string.Empty);

            if (string.IsNullOrEmpty(accessToken))
            {
                return new Window(new NavigationPage(new LoginPage(_apiService, _validator)));
            }

            return new Window(new AppShell(_apiService, _validator));
        }

    }
}