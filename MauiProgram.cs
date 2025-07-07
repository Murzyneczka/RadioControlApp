using CommunityToolkit.Mvvm.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RadioControlApp.Services;
using RadioControlApp.ViewModels;

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

        // Konfiguracja appsettings.json
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        // Rejestracja serwisów
        builder.Services.AddHttpClient<IRadioDeviceService, RadioApiService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["RadioDeviceApi:BaseUrl"]);
            client.Timeout = TimeSpan.FromSeconds(int.Parse(builder.Configuration["RadioDeviceApi:TimeoutSeconds"]));
        });
        builder.Services.AddSingleton<ILocalStorageService, LocalStorageService>();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MauiProgram).Assembly));
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<IRConfigurationViewModel>();

        return builder.Build();
    }
}