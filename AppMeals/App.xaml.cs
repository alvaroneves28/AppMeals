using AppMeals.Pages;
using AppMeals.Services;
using AppMeals.Validations;

namespace AppMeals
{
    public partial class App : Application
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        public App(ApiService apiService, IValidator validator)
        {
            InitializeComponent();
            _apiService = apiService;
            _validator = validator;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new RegisterPage(_apiService, _validator)));
        }
    }
}