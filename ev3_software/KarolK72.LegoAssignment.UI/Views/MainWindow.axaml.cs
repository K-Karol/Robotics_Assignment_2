using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace KarolK72.LegoAssignment.UI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ActivatorUtilities.CreateInstance((IServiceProvider)this.FindResource(typeof(IServiceProvider))!, typeof(ViewModels.MainWindowViewModel));
        }
    }
}