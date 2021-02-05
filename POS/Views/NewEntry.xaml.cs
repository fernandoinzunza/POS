using Microsoft.Toolkit.Uwp.UI.Controls;
using POS.Models;
using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class NewEntry : Page
    {
        InventoryViewModel vm;
        public DataGrid data;
        public EntradaAlmacen seleccionado = new EntradaAlmacen();
        public NewEntry()
        {
            this.InitializeComponent();
            vm = new InventoryViewModel();
            this.DataContext = vm;
            dataGrid.DataContext = vm;
            data = dataGrid;
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var seleccionado = data.SelectedItem;
            Button route = e.OriginalSource as Button;
            var s = route.DataContext as EntradaAlmacen;
            vm.ListaTemporal.Remove(s);
            vm.TotalCompra -= s.CostoTotal;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            seleccionado = sender as EntradaAlmacen;
        }

        private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            seleccionado = sender as EntradaAlmacen;
        }
    }
}
