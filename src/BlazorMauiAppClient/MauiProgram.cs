using AppCore.Extensions;
using BlazorMauiAppClient.Models;
using Microsoft.Extensions.Logging;

namespace BlazorMauiAppClient
{
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
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Services.AddAppCore(builder.Configuration);
            builder.Services.AddSingleton<SharedState>();

            return builder.Build();
        }
    }
}