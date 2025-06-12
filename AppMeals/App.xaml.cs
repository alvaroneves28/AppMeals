using AppMeals.Pages;
using AppMeals.Services;

namespace AppMeals
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        public App(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new RegisterPage(_apiService)));
        }
    }
}