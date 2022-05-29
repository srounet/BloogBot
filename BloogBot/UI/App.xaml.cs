using System;
using System.Diagnostics;
using System.Windows;

namespace BloogBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Debugger.Launch();

            var mainWindow = new MainWindow();
            Current.MainWindow = mainWindow;
            mainWindow.Closed += (sender, args) => { Environment.Exit(0); };
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}