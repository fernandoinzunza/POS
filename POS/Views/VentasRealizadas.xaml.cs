using Microsoft.Toolkit.Uwp.UI.Controls;
using POS.CustomControls;
using POS.Models;
using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class VentasRealizadas : Page
    {
        SellViewModel vm;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public VentasRealizadas()
        {
            this.InitializeComponent();
            vm = new SellViewModel();
            this.DataContext = vm;

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var tienePermiso = (bool)localSettings.Values["RealizarCancelacion"];
            if(tienePermiso)
            {
                var boton = sender as Button;
                var carrito = boton.DataContext as Carrito;
                var dialog = new CustomContentDialog();
                dialog.Title = "Cancelacion de articulo";
                dialog.Folio = carrito.Folio.ToString();
                dialog.Articulo = carrito.DescripcionArticulo;
                dialog.Cantidad = carrito.Cantidad;
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    if (dialog.Cantidad > carrito.Cantidad)
                    {
                        var contentDialog = new ContentDialog();
                        contentDialog.Content = "No puedes cancelar mas cantidad de la que se compro!";
                        contentDialog.Title = "Error";
                        contentDialog.PrimaryButtonText = "Enterado";
                        await contentDialog.ShowAsync();
                    }
                    else if (dialog.Cantidad <= 0)
                    {
                        var contentDialog = new ContentDialog();
                        contentDialog.Content = "Agrega una cantidad mayor que 0!";
                        contentDialog.Title = "Error";
                        contentDialog.PrimaryButtonText = "Enterado";
                        await contentDialog.ShowAsync();
                    }
                    else
                    {

                        carrito.RazonCancelado = dialog.Text;
                        if (carrito.Cantidad == dialog.Cantidad)
                            carrito.Cancelado = "Parcialmente";

                        else
                        {
                            carrito.Cancelado = "Totalmente";
                        }
                        switch (dialog.TipoCancelacion)
                        {
                            case "Devolucion":
                                {
                                    var l = await vm.GetDevolucionesAsync(carrito.Folio.ToString());
                                    var listaAuxiliar = new List<Devolucion>();
                                    var yaHuboCancelacion = l.Where(b => b.Folio == Convert.ToInt32(dialog.Folio) && b.IdCarrito == carrito.Id).Count();

                                    if (yaHuboCancelacion == 0)
                                    {
                                        var cantidad = dialog.Cantidad;
                                        carrito.Cantidad -= dialog.Cantidad;
                                        var resta = carrito.PrecioPublico - (carrito.PrecioPublico * (carrito.PorcentajeDescuento / 100));
                                        carrito.SubTotal = carrito.SubTotal - (resta * dialog.Cantidad);

                                        await vm.CancelarTotalmenteAsync(carrito, dialog.Cantidad, dialog.Text, dialog.TipoCancelacion);
                                        var aux = vm.VentasSeparadas;
                                        /*
                                        await Task.Run(() =>
                                        {

                                            foreach (var v in aux)
                                            {
                                                foreach (var s in v.Value)
                                                {
                                                    if (s.Folio == carrito.Folio && s.Id == carrito.Id)
                                                    {
                                                        s.Cantidad -=cantidad;
                                                        s.SubTotal = carrito.SubTotal - (carrito.PrecioPublico * cantidad);
                                                        s.SubTotalSinIva = s.SubTotal / 1.16;
                                                    }
                                                }
                                            }

                                        });
                                        dataGrid.ItemsSource = aux;
                                        */
                                        Frame.Navigate(typeof(VentasRealizadas));
                                    }
                                    else
                                    {
                                        var md = new MessageDialog("Ya hubo una cancelacion para este articulo.\n Ya no puedes reponerlo ni devolverlo", "Mensaje del Sistema");
                                        await md.ShowAsync();
                                    }



                                    break;
                                }
                            case "Reposicion":
                                {
                                    var l = await vm.GetDevolucionesAsync(carrito.Folio.ToString());
                                    var listaAuxiliar = new List<Devolucion>();
                                    var yaHuboCancelacion = l.Where(b => b.Folio == Convert.ToInt32(dialog.Folio) && b.IdCarrito == carrito.Folio).Count();
                                    if (yaHuboCancelacion == 0)
                                    {
                                        foreach (var d in l)
                                        {
                                            var folio = d.Folio;
                                            var cart = d.IdCarrito;
                                            var existe = listaAuxiliar.Where(b => b.Folio == folio && b.IdCarrito == cart).Count();
                                            if (existe == 0)
                                            {
                                                listaAuxiliar.Add(d);
                                            }
                                            else
                                            {
                                                var devolucion = listaAuxiliar.Where(b => b.Folio == folio && b.IdCarrito == cart).First();
                                                devolucion.Cantidad += d.Cantidad;
                                                devolucion.Perdida += d.Perdida;
                                                devolucion.PerdidaSivIva = devolucion.Perdida / 1.16;
                                            }

                                        }
                                        var devolucionEspecifica = listaAuxiliar.Where(b => b.Folio == carrito.Folio && b.IdCarrito == carrito.Id).FirstOrDefault();
                                        var hayDevolucion = listaAuxiliar.Where(b => b.Folio == carrito.Folio && b.IdCarrito == carrito.Id).Count();
                                        if (hayDevolucion > 0)
                                        {
                                            if (devolucionEspecifica.Cantidad < carrito.Cantidad)
                                            {
                                                await vm.ReponerMercancia(carrito, dialog.Cantidad, dialog.Text, dialog.TipoCancelacion);

                                            }
                                            else
                                            {
                                                var md = new MessageDialog("Ya se repuso la cantidad comprada en su totalidad", "Mensaje del Sistema");
                                                await md.ShowAsync();
                                            }
                                        }
                                        else
                                        { await vm.ReponerMercancia(carrito, dialog.Cantidad, dialog.Text, dialog.TipoCancelacion); }
                                    }

                                    else
                                    {
                                        var md = new MessageDialog("Ya hubo una cancelacion para este articulo.\n Ya no puedes reponerlo ni devolverlo", "Mensaje del Sistema");
                                        await md.ShowAsync();

                                    }
                                    break;
                                }
                        }
                        if (carrito.Cantidad <= dialog.Cantidad)
                        {


                        }

                    }
                }
            }
            else
            {
                var md = new MessageDialog("No tienes permisos para cancelar!!", "Mensaje del Sistema");
                await md.ShowAsync();
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private async void btnDevoluciones_Click(object sender, RoutedEventArgs e)
        {
            var contentDialog = new DevolucionesContentDialog();
            var venta = (sender as Button).DataContext;
            var carrito = (KeyValuePair<Carrito, ObservableCollection<Carrito>>)venta;
            var l = await vm.GetDevolucionesAsync(carrito.Key.Folio.ToString());
            var listaAuxiliar = new List<Devolucion>();
            foreach (var d in l)
            {
                var folio = d.Folio;
                var cart = d.IdCarrito;
                var existe = listaAuxiliar.Where(b => b.Folio == folio && b.IdCarrito == cart && b.TipoCancelacion == d.TipoCancelacion).Count();
                if (existe == 0)
                {
                    listaAuxiliar.Add(d);
                }
                else
                {
                    var devolucion = listaAuxiliar.Where(b => b.Folio == folio && b.IdCarrito == cart && b.TipoCancelacion == d.TipoCancelacion).First();
                    devolucion.Cantidad += d.Cantidad;
                    devolucion.Perdida += d.Perdida;
                    devolucion.PerdidaSivIva = devolucion.Perdida / 1.16;
                }

            }
            var lista = new ObservableCollection<Devolucion>(listaAuxiliar);
            contentDialog.ListaDevoluciones = lista;
            await contentDialog.ShowAsync();
        }

        private void dataGrid_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {

        }

        private void total_Loaded(object sender, RoutedEventArgs e)
        {

            double total = 0;
            double totalConIva = 0;
            var txt = sender as TextBox;
            var carrito = (KeyValuePair<Carrito, ObservableCollection<Carrito>>)txt.DataContext;
            foreach (var v in vm.VentasSeparadas)
            {
                if (carrito.Key.Folio == v.Key.Folio)
                {
                    foreach (var lista in v.Value)
                    {
                        total += lista.SubTotal;
                        totalConIva += lista.SubTotalSinIva;
                    }
                }

            }
            txt.Text = totalConIva.ToString();
        }

        private void totalConIva_Loaded(object sender, RoutedEventArgs e)
        {
            double total = 0;
            double totalConIva = 0;
            var txt = sender as TextBox;
            var carrito = (KeyValuePair<Carrito, ObservableCollection<Carrito>>)txt.DataContext;
            foreach (var v in vm.VentasSeparadas)
            {
                if (carrito.Key.Folio == v.Key.Folio)
                {
                    foreach (var lista in v.Value)
                    {
                        total += lista.SubTotal;
                        totalConIva += lista.SubTotalSinIva;
                    }
                }

            }
            txt.Text = total.ToString();
        }
    }
}
