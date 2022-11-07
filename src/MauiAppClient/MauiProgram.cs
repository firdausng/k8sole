using AppCore.Extensions;
using CommunityToolkit.Maui;
using MauiAppClient.ViewModels;
using MauiAppClient.Views;

namespace MauiAppClient
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

            builder.Services.AddSingleton<Application>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainPage2>();
            builder.Services.AddTransient<NewPage2>();
            builder.Services.AddTransient<MainViewModel>();
            builder.Services.AddTransient<K8sPage>();
            builder.Services.AddAppCore(builder.Configuration);

            return builder.Build();
        }
    }
}