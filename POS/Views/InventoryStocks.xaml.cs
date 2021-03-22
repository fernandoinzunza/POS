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
    public sealed partial class InventoryStocks : Page
    {
        InventoryViewModel vm;
        public InventoryStocks()
        {
            InitializeComponent();
            vm = new InventoryViewModel();
            DataContext = vm;
        }

        private void btnDetalles_Click(object sender, RoutedEventArgs e)
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

        private void TextBlock_KeyUp(object sender, KeyRoutedEventArgs e)
        {

        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (buscarStock.Text == "")
            {
                dataGrid.ItemsSource = vm.Disponibilidad;
            }
            else
            {
                Dictionary<KeyValuePair<Articulo,string>, List<CajaDispArticulo>> nuevo = vm.Disponibilidad.Where(b => b.Key.Key.Clave.Contains(buscarStock.Text)).ToDictionary(b => b.Key, b => b.Value);
                dataGrid.ItemsSource = nuevo;
            }
        }
    }
}
