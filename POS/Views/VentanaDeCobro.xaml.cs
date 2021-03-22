using Microsoft.Toolkit.Uwp.UI.Controls;
using POS.Models;
using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
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
    public sealed partial class VentanaDeCobro : Page
    {
        SellViewModel vm;
        List<CajaDispArticulo> lista;
        Carrito carrito;
        public VentanaDeCobro()
        {
            this.InitializeComponent();
            vm = new SellViewModel();
            this.DataContext = vm;
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            lector.IsChecked = true;
            teclado.IsChecked = false;
            //efectivo.IsChecked = true;
          
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(DragGrid);
            dataGrid.SelectionMode = DataGridSelectionMode.Single;
            vm.CajasModificadasArticulos = new List<List<CajaDispArticulo>>();
            cajaSug.Focus(FocusState.Programmatic);
            // txtFolio.Text = vm.GetFolio().ToString();
        }
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            DragGrid.Height = sender.Height;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && lector.IsChecked == true)
            {
                var lista = vm.Articulos.Where(b => b.Clave == cajaSug.Text).FirstOrDefault();
                var carrito = new Carrito();
                if (lista != null)
                {
                    var articulo = lista;
                    carrito.ClaveArticulo = articulo.Clave;
                    carrito.DescripcionArticulo = articulo.Descripcion;
                    carrito.Cantidad = 0;
                    carrito.SubTotal = 0;
                    carrito.PrecioPublico = articulo.PrecioPublico;
                    carrito.IdDescuento = articulo.IdDescuento;
                    var descuento = vm.Descuentos.Where(b => b.NoDescuento == articulo.IdDescuento).First();
                    carrito.PorcentajeDescuento = descuento.Porcentaje;
                    carrito.NombreDescuento = articulo.NombreDescuento;
                    carrito.PrecioAlCosto = articulo.PrecioAlCosto;
                    carrito.Utilidad = 0;
                    var existe = vm.Compra.Where(b => b.ClaveArticulo == cajaSug.Text).ToList().Count;
                    if (existe == 0)
                    {
                        vm.Compra.Add(carrito);
                        cajaSug.Text = "";
                        dataGrid.SelectedIndex = vm.Compra.Count - 1;
                        dataGrid.CurrentColumn = dataGrid.Columns[2];
                        dataGrid.Focus(FocusState.Keyboard);
                        dataGrid.BeginEdit();
                    }
                    else
                    {
                        var item = vm.Compra.Where(b => b.ClaveArticulo == cajaSug.Text).First();
                        var indice = vm.Compra.IndexOf(item);
                        cajaSug.Text = "";
                        dataGrid.SelectedIndex = indice;
                        dataGrid.CurrentColumn = dataGrid.Columns[2];
                        dataGrid.Focus(FocusState.Keyboard);
                        dataGrid.BeginEdit();

                    }

                }
                else
                {
                    cajaSug.ItemsSource = vm.Articulos.Where(b => b.Descripcion.ToUpper().Contains(cajaSug.Text.ToUpper()) || b.Clave.Contains(cajaSug.Text)).ToList();
                }

            }
            else if (teclado.IsChecked == true)
            {
                cajaSug.ItemsSource = vm.Articulos.Where(b => b.Descripcion.ToUpper().Contains(cajaSug.Text.ToUpper()) || b.Clave.Contains(cajaSug.Text)).ToList();
            }

        }

        private void cajaSug_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {

        }

        private void cajaSug_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {

        }

        private void cajaSug_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            carrito = new Carrito();
            if (args.ChosenSuggestion != null)
            {
                var articulo = (args.ChosenSuggestion as Articulo);
                carrito.ClaveArticulo = articulo.Clave;
                carrito.DescripcionArticulo = articulo.Descripcion;
                carrito.IdDescuento = articulo.IdDescuento;
                var descuento = vm.Descuentos.Where(b => b.NoDescuento == carrito.IdDescuento).First();
                carrito.PorcentajeDescuento = descuento.Porcentaje;
                carrito.NombreDescuento = descuento.NombreDescuento;
                carrito.Cantidad = 0;
                carrito.SubTotal = 0;
                carrito.PrecioPublico = articulo.PrecioPublico;
                carrito.PrecioAlCosto = articulo.PrecioAlCosto;
                carrito.Utilidad = 0;
                var existe = vm.Compra.Where(b => b.ClaveArticulo == cajaSug.Text).ToList().Count;
                if (existe == 0)
                {
                    vm.Compra.Add(carrito);
                    cajaSug.Text = "";
                    dataGrid.SelectedIndex = vm.Compra.Count - 1;
                    dataGrid.CurrentColumn = dataGrid.Columns[2];
                    dataGrid.Focus(FocusState.Keyboard);
                    dataGrid.BeginEdit();
                }
                else
                {
                    var item = vm.Compra.Where(b => b.ClaveArticulo == cajaSug.Text).First();
                    var indice = vm.Compra.IndexOf(item);
                    cajaSug.Text = "";
                    dataGrid.SelectedIndex = indice;
                    dataGrid.CurrentColumn = dataGrid.Columns[2];
                    dataGrid.Focus(FocusState.Keyboard);
                    dataGrid.BeginEdit();

                }


            }
            else
            {
                if (cajaSug.Text != "")
                {
                    var articulo = vm.Articulos.Where(b => b.Descripcion.ToUpper().Contains(cajaSug.Text.ToUpper())).First();
                    carrito.ClaveArticulo = articulo.Clave;
                    carrito.DescripcionArticulo = articulo.Descripcion;
                    carrito.IdDescuento = articulo.IdDescuento;
                    var descuento = vm.Descuentos.Where(b => b.NoDescuento == carrito.IdDescuento).First();
                    carrito.PorcentajeDescuento = descuento.Porcentaje;
                    carrito.NombreDescuento = descuento.NombreDescuento;
                    carrito.Cantidad = 0;
                    carrito.SubTotal = 0;
                    carrito.PrecioPublico = articulo.PrecioPublico;
                    carrito.PrecioAlCosto = articulo.PrecioAlCosto;
                    carrito.Utilidad = 0;
                    var existe = vm.Compra.Where(b => b.ClaveArticulo == cajaSug.Text).ToList().Count;
                    if (existe == 0)
                    {
                        vm.Compra.Add(carrito);
                        cajaSug.Text = "";
                        dataGrid.SelectedIndex = vm.Compra.Count - 1;
                        dataGrid.CurrentColumn = dataGrid.Columns[2];
                        dataGrid.Focus(FocusState.Keyboard);
                        dataGrid.BeginEdit();
                    }
                    else
                    {
                        var item = vm.Compra.Where(b => b.ClaveArticulo == cajaSug.Text).First();
                        var indice = vm.Compra.IndexOf(item);
                        cajaSug.Text = "";
                        dataGrid.SelectedIndex = indice;
                        dataGrid.CurrentColumn = dataGrid.Columns[2];
                        dataGrid.Focus(FocusState.Keyboard);
                        dataGrid.BeginEdit();

                    }
                }
                else
                    cajaSug.IsSuggestionListOpen = true;
            }
        }

        private void dataGrid_CellEditEnded(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridCellEditEndedEventArgs e)
        {
            var indice = sender as DataGrid;
            var index = indice.SelectedIndex;
            var nuevo = indice.SelectedItem as Carrito;
            nuevo.SubTotal = nuevo.Cantidad * nuevo.PrecioPublico;
            double subtotalConDescuento = nuevo.SubTotal - (nuevo.SubTotal * (nuevo.PorcentajeDescuento / 100));
            nuevo.SubTotal = subtotalConDescuento;
            vm.DisponibilidadStockAsync(nuevo.ClaveArticulo);
            List<CajaDispArticulo> menosStock = new List<CajaDispArticulo>();
            double stockTotal = 0;
            double utilidad = 0;
            foreach (var c in vm.Lista)
            {
                stockTotal += c.StockTotal;
            }
            if (stockTotal >= nuevo.Cantidad)
            {
                /*
                var auxCantidad = nuevo.Cantidad;
                foreach(var c in vm.Lista)
                {
                    var carrito = new Carrito();
                    if(auxCantidad <=c.StockTotal)
                    {
                        c.StockTotal -= auxCantidad;
                        carrito.Cantidad = auxCantidad;
                        carrito.ClaveArticulo = c.ClaveArticulo;
                        carrito.DescripcionArticulo = nuevo.DescripcionArticulo;
                        carrito.MetodoDePago = "";
                        carrito.Folio = 0;
                        carrito.PrecioAlCosto = c.PrecioUnitarioSinIVA;
                        carrito.PrecioPublico = nuevo.PrecioPublico;
                        carrito.Utilidad = ((nuevo.PrecioPublico/1.16) - c.PrecioUnitarioSinIVA)*auxCantidad;
                        vm.CompraPorUtilidad.Add(carrito);
                        if (auxCantidad <= c.StockIndividual)
                        {
                            c.StockIndividual -= auxCantidad;
                        }
                        else if(nuevo.Cantidad > c.StockIndividual)
                        {
                            var cantidadACajas = (int)Math.Floor(nuevo.Cantidad / c.Capacidad);
                            var sobra = auxCantidad % c.Capacidad;
                            utilidad = (cantidadACajas * c.Capacidad) * c.PrecioUnitarioSinIVA + sobra * c.PrecioUnitarioSinIVA;

                            c.CajasDisponibles -= cantidadACajas;
                            if(sobra > c.StockIndividual)
                            {
                                c.CajasDisponibles--;
                                c.StockIndividual = c.Capacidad - sobra;
                            }

                        }
                        break;
                    }
                    else if(auxCantidad >c.StockTotal)
                    {
                        auxCantidad = auxCantidad - c.StockTotal;
                        carrito.Utilidad = ((nuevo.PrecioPublico / 1.16) - c.PrecioUnitarioSinIVA) * c.StockTotal;
                        carrito.SubTotal = c.StockTotal * nuevo.PrecioPublico;
                        carrito.Cantidad = c.StockTotal;
                        c.StockTotal = 0;
                        c.CajasDisponibles = 0;
                        c.StockIndividual = 0;
                        carrito.ClaveArticulo = c.ClaveArticulo;
                        carrito.DescripcionArticulo = nuevo.DescripcionArticulo;
                        carrito.MetodoDePago = "";
                        carrito.Folio = 0;
                        carrito.PrecioAlCosto = c.PrecioUnitarioSinIVA;
                        carrito.PrecioPublico = nuevo.PrecioPublico;
                        vm.CompraPorUtilidad.Add(carrito);
                    }
                }
                */
                vm.Compra.RemoveAt(index);
                vm.Compra.Insert(index, nuevo);
                vm.TotalCompra = 0;
                foreach (var c in vm.Compra)
                {
                    vm.TotalCompra += c.SubTotal;
                }
                cajaSug.Focus(FocusState.Programmatic);
            }
            else
            {
                var md = new MessageDialog("No hay inventario suficiente para la venta solicitada", "Mensaje del Sistema");
                md.ShowAsync();
                dataGrid.SelectedIndex = vm.Compra.Count - 1;
                dataGrid.CurrentColumn = dataGrid.Columns[2];
                dataGrid.Focus(FocusState.Keyboard);
                dataGrid.BeginEdit();


            }


        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var row = sender;
            var articulo = (row as Button).DataContext as Carrito;
            vm.Compra.Remove(articulo);
            vm.CompraPorUtilidad = new List<Carrito>();
            vm.CajasModificadasArticulos = new List<List<CajaDispArticulo>>();
            foreach (var itemCompra in vm.Compra)
            {
                var listaCajasDisp = await vm.DisponibilidadArticulo(itemCompra.ClaveArticulo);
                double stockTotal = 0;
                double utilidad = 0;
                foreach (var caja in listaCajasDisp)
                {
                    stockTotal += caja.StockTotal;
                }
                foreach (var caja in listaCajasDisp)
                {
                    if (stockTotal >= itemCompra.Cantidad)
                    {
                        var auxCantidad = itemCompra.Cantidad;
                        var carrito = new Carrito();
                        if (auxCantidad <= caja.StockTotal)
                        {
                            caja.StockTotal -= auxCantidad;
                            carrito.Cantidad = auxCantidad;
                            carrito.ClaveArticulo = caja.ClaveArticulo;
                            carrito.DescripcionArticulo = itemCompra.DescripcionArticulo;
                            carrito.MetodoDePago = "";
                            carrito.Folio = 0;
                            carrito.PrecioAlCosto = caja.PrecioUnitarioSinIVA;
                            carrito.PrecioPublico = itemCompra.PrecioPublico;
                            carrito.Utilidad = ((itemCompra.PrecioPublico / 1.16) - caja.PrecioUnitarioSinIVA) * auxCantidad;
                            vm.CompraPorUtilidad.Add(carrito);
                            if (auxCantidad <= caja.StockIndividual)
                            {
                                caja.StockIndividual -= auxCantidad;
                            }
                            else if (itemCompra.Cantidad > caja.StockIndividual)
                            {
                                var cantidadACajas = (int)Math.Floor(itemCompra.Cantidad / caja.Capacidad);
                                var sobra = auxCantidad % caja.Capacidad;
                                utilidad = (cantidadACajas * caja.Capacidad) * caja.PrecioUnitarioSinIVA + sobra * caja.PrecioUnitarioSinIVA;

                                caja.CajasDisponibles -= cantidadACajas;
                                if (sobra > caja.StockIndividual)
                                {
                                    caja.CajasDisponibles--;
                                    caja.StockIndividual = caja.Capacidad - sobra;
                                }

                            }
                            break;
                        }
                        else if (auxCantidad > caja.StockTotal)
                        {
                            auxCantidad = auxCantidad - caja.StockTotal;
                            carrito.Utilidad = ((itemCompra.PrecioPublico / 1.16) - caja.PrecioUnitarioSinIVA) * caja.StockTotal;
                            carrito.SubTotal = caja.StockTotal * itemCompra.PrecioPublico;
                            carrito.Cantidad = caja.StockTotal;
                            caja.StockTotal = 0;
                            caja.CajasDisponibles = 0;
                            caja.StockIndividual = 0;
                            carrito.ClaveArticulo = caja.ClaveArticulo;
                            carrito.DescripcionArticulo = itemCompra.DescripcionArticulo;
                            carrito.MetodoDePago = "";
                            carrito.Folio = 0;
                            carrito.PrecioAlCosto = caja.PrecioUnitarioSinIVA;
                            carrito.PrecioPublico = itemCompra.PrecioPublico;
                            vm.CompraPorUtilidad.Add(carrito);
                        }
                    }
                }
                vm.CajasModificadasArticulos.Add(listaCajasDisp);
            }

            vm.TotalCompra = 0;
        }

        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.EditingElement is TextBox textbox)
            {
                textbox.Focus(FocusState.Programmatic);
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cajaSug.Focus(FocusState.Programmatic);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}


