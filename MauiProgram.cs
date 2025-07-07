using FluentValidation;
//cursor/design-remote-control-application-functions-d9c3
using Microsoft.Extensions.DependencyInjection;
using RadioControlApp.Models;
using RadioControlApp.Services;
using RadioControlApp.ViewModels;
using RadioControlApp.Views;

namespace RadioControlApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Rejestracja serwisów
        builder.Services.AddHttpClient<IRadioDeviceService, RadioApiService>(client =>
        {

            /*var baseUrl = "https://radio-device-api.example.com/";
            var timeoutSeconds = 30;*/

            var baseUrl = builder.Configuration["RadioDeviceApi:BaseUrl"] ?? "https://radio-device-api.example.com/";
            var timeoutSeconds = int.Parse(builder.Configuration["RadioDeviceApi:TimeoutSeconds"] ?? "30");
            
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        });
        
        builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();

        // Rejestracja walidatorów
        builder.Services.AddScoped<IValidator<IRCode>, IRCodeValidator>();
        builder.Services.AddScoped<IValidator<FMStation>, FMStationValidator>();

        // Rejestracja ViewModels
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<IRConfigurationViewModel>();
        builder.Services.AddTransient<FMConfigurationViewModel>();

        // Rejestracja stron
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<IRConfigurationPage>();
        builder.Services.AddTransient<FMConfigurationPage>();

        return builder.Build();
    }
}
