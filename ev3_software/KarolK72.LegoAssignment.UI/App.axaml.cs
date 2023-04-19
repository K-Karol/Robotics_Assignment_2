using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using KarolK72.LegoAssignment.Library;
using KarolK72.LegoAssignment.UI.ViewModels;
using KarolK72.LegoAssignment.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KarolK72.LegoAssignment.UI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            IServiceCollection serviceCollection = new ServiceCollection();
            Configure(serviceCollection);
            this.Resources[typeof(IServiceProvider)] = serviceCollection.BuildServiceProvider();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
        private void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IEV3CommunicationService, ConcreteEV3CommunicationService>();
        }
    }
}