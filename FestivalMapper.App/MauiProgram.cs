using FestivalMapper.App.Infrastructure;
using FestivalMapper.App.Interfaces;
using FestivalMapper.App.Services;
using Microsoft.Extensions.Logging;

namespace FestivalMapper.App
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

            // resolve a stable app data folder
            var root = FileSystem.AppDataDirectory;
            var repoPath = Path.Combine(root, "festivals");
            var repo = new JsonFestivalRepositroy(repoPath);

            // dependency injection registrations
            builder.Services.AddSingleton<IFestivalRepository>(_ => repo);
            builder.Services.AddScoped<IFestivalService, FestivalService>();
            builder.Services.AddScoped<FestivalSelectionState>();

            // my services
            builder.Services.AddSingleton<ImagePickerService>();
            builder.Services.AddSingleton<MapStorageService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
