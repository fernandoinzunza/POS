using Microsoft.Toolkit.Uwp.UI.Controls;
using POS.Models;
using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ComprasAlmacen : Page
    {
        InventoryViewModel vm;
        public ComprasAlmacen()
        {
            this.InitializeComponent();
            vm = new InventoryViewModel();
            this.DataContext = vm;
            


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;
            while (!(obj is DataGridRow) && obj != null) obj = VisualTreeHelper.GetParent(obj);

            if (obj is DataGridRow)
            {
                if ((obj as DataGridRow).DetailsVisibility == Visibility.Visible)
                {
                    (obj as DataGridRow).DetailsVisibility = Visibility.Collapsed;

                }
                else
                {
                    (obj as DataGridRow).DetailsVisibility = Visibility.Visible;
                }
            }

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (buscarCompra.Text == "")
            {
                dataGrid.ItemsSource = vm.ComprasAlmacen;
            }
            else
            {
                Dictionary<EntradaAlmacen, List<EntradaAlmacen>> nuevo = vm.ComprasAlmacen.Where(b => b.Key.NoFactura.Contains(buscarCompra.Text)).ToDictionary(b => b.Key, b => b.Value);
                dataGrid.ItemsSource = nuevo;
            }
        }
    }
}
