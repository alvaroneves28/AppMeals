﻿using AppMeals.Services;
using AppMeals.Validations;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AppMeals
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            

            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<FavoritesService>();
            builder.Services.AddSingleton<IValidator, Validator>();
            builder.Services.AddHttpClient();
            return builder.Build();
        }

    }


}
