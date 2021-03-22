using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        StartPageViewModel vm;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public StartPage()
        {
            this.InitializeComponent();
             vm = new StartPageViewModel(new NavigationService());
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            Window.Current.SetTitleBar(BackgroundElement);
            this.DataContext = vm;
            btn2.IsEnabled = (bool)localSettings.Values["VentanaDeCobro"];
            string puesto = (string)localSettings.Values["Puesto"];
            if (puesto != "Administrador")
                btn6.IsEnabled = false;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
           BackgroundElement.Height = sender.Height;
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
           Frame.Navigate(typeof(VentanaDeCobro));
        }
    }
}
