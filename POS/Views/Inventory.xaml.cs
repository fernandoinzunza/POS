using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class Inventory : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public Inventory()
        {
            InitializeComponent();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(DragGrid);
            nuevaEntrada.IsEnabled = (bool)localSettings.Values["NuevaEntrada"];
            nuevaSalida.IsEnabled = (bool)localSettings.Values["NuevaSalida"];
            entradaPorReposicion.IsEnabled = (bool)localSettings.Values["EntradaPorReposicion"];
            verStocks.IsEnabled = (bool)localSettings.Values["StockDisponible"];
            bitacoraEntradas.IsEnabled = (bool)localSettings.Values["BitacoraEntradas"];
            bitacoraSalidas.IsEnabled = (bool)localSettings.Values["BitacoraDeSalidas"];
            
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            DragGrid.Height = sender.Height;
        }

        private void rootNavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            Frame.Navigate(typeof(StartPage));
        }

        private void rootNavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            Microsoft.UI.Xaml.Controls.NavigationViewItem nvi = args.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem;
            if (nvi.Tag.ToString() != "")
            {
                contentFrame.HorizontalAlignment = HorizontalAlignment.Stretch;
                contentFrame.VerticalAlignment = VerticalAlignment.Stretch;
            }
            switch (nvi.Tag.ToString())
            {
                case "InventoryStocks":
                    {
                        contentFrame.Navigate(typeof(InventoryStocks));
                        break;
                    }
                case "NewEntry":
                    {
                        contentFrame.Navigate(typeof(NewEntry));
                        break;
                    }
                case "ComprasAlmacen":
                    {
                        contentFrame.Navigate(typeof(ComprasAlmacen));
                        break;
                    }
                case "SalidaAlmacen":
                    {
                        contentFrame.Navigate(typeof(SalidaAlmacen));
                        break;
                    }
                case "ListaSalidas":
                    {
                        contentFrame.Navigate(typeof(ListaSalidas));
                        break;
                    }
                case "EntradaPorReposicion":
                    {
                        contentFrame.Navigate(typeof(EntradaPorReposicion));
                        break;
                    }
                case "ListaReposiciones":
                    {
                        contentFrame.Navigate(typeof(ListaReposiciones));
                        break;
                    }
            }
        }

        private void rootNavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            var navView = sender as Microsoft.UI.Xaml.Controls.NavigationView;
            var rootGrid = VisualTreeHelper.GetChild(navView, 0) as Grid;

            // SDK 18362 (1903)
            // SDK 17763 (1809)

                // Find the back button.
                var paneToggleButtonGrid = VisualTreeHelper.GetChild(rootGrid, 0) as Grid;
                var buttonHolderGrid = VisualTreeHelper.GetChild(paneToggleButtonGrid, 1) as Grid;
                var navigationViewBackButton = VisualTreeHelper.GetChild(buttonHolderGrid, 0) as Button;
                navigationViewBackButton.Background = new SolidColorBrush(Color.FromArgb(255, 50, 168, 82));
        }
    }
}
