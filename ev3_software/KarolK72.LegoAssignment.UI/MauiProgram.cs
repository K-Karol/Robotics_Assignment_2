using CommunityToolkit.Maui;
using KarolK72.LegoAssignment.Library;
using Microsoft.Extensions.Logging;

namespace KarolK72.LegoAssignment.UI
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

            ConfigureService(builder.Services);

#if DEBUG
		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }

        /// <summary>
        /// Configures the dependency injection container (IoC).
        /// </summary>
        /// <param name="serviceCollection"></param>
        private static void ConfigureService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(loggingBuilder =>
            {
#if DEBUG
                loggingBuilder.AddFilter("KarolK72.LegoAssignment", LogLevel.Debug);
#else
                loggingBuilder.AddFilter("KarolK72.LegoAssignment", LogLevel.Information);
#endif
            });

            serviceCollection.AddSingleton<IEV3CommunicationService, ConcreteEV3CommunicationService>(); // registering the concrete implementation of the IEV3CommunicationService

            //view models
            serviceCollection.AddSingleton<ViewModels.MainViewModel>(); // there is only one *page* and the view model is shared with both MainViewDesktop and MainViewMobile so it is reg. as a singleton

            //pages - registering a page means it will get deps. injected.
            serviceCollection.AddTransient<Views.MainViewLoader>();
            serviceCollection.AddTransient<Views.MainView>();
            serviceCollection.AddTransient<Views.MainViewDesktop>();
            serviceCollection.AddTransient<Views.MainViewMobile>();

        }
    }
}