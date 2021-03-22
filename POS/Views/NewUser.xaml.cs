using POS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class NewUser : Page
    {
        UsersViewModel vm;
        public NewUser()
        {
            this.InitializeComponent();
            vm = new UsersViewModel();
            this.DataContext = vm;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
           foreach(Microsoft.UI.Xaml.Controls.TreeViewNode nodo in tree.SelectedNodes)
            {
                var padre = nodo.Parent;
                if(nodo.Parent.Content !=null)
                {
                    if (nodo.Parent.Content.ToString() == "Articulos")
                    {
                        if (nodo.Content.ToString() == "Dar de alta")
                            vm.NuevoArticulo = true;
                        else if (nodo.Content.ToString() == "Modificar")
                            vm.ModificarArticulo = true;
                        else if (nodo.Content.ToString() == "Eliminar")
                            vm.EliminarArticulo = true;
                        else if (nodo.Content.ToString() == "Agregar un descuento")
                            vm.NuevoDescuento = true;
                    }
                    else if (nodo.Parent.Content.ToString() == "Entradas de almacen")
                    {
                        if (nodo.Content.ToString() == "Agregar una entrada")
                            vm.NuevaEntrada = true;
                        else if (nodo.Content.ToString() == "Agregar una entrada por reposicion")
                            vm.EntradaPorReposicion = true;
                        else if (nodo.Content.ToString() == "Ver bitacora de entradas")
                            vm.BitacoraEntradas = true;
                        else if (nodo.Content.ToString() == "Ver stocks disponibles")
                            vm.StockDisponible = true;
                    }
                    else if (nodo.Parent.Content.ToString() == "Salidas de almacen")
                    {
                        if (nodo.Content.ToString() == "Agregar una salida")
                            vm.NuevaSalida = true;
                        else if (nodo.Content.ToString() == "Ver bitacora de salidas")
                            vm.BitacoraDeSalidas = true;
                    }
                    else if (nodo.Parent.Content.ToString() == "Proveedores")
                    {
                        if (nodo.Content.ToString() == "Modificar un proveedor")
                            vm.EditarProveedor = true;
                        if (nodo.Content.ToString() == "Eliminar un proveedor")
                            vm.EliminarProveedor = true;
                        if (nodo.Content.ToString() == "Agregar un proveedor")
                            vm.NuevoProveedor = true;
                    }
                    else if (nodo.Parent.Content.ToString() == "Ventana de cobro")
                    {
                        if (nodo.Content.ToString() == "Acceso a la ventana de cobro")
                            vm.VentanaDeCobro = true;
                    }
                    else if (nodo.Parent.Content.ToString() == "Ventas")
                    {
                        if (nodo.Content.ToString() == "Realizar una cancelacion")
                            vm.RealizarCancelacion = true;
                        else if (nodo.Content.ToString() == "Generar reporte de devoluciones")
                            vm.ReporteDeDevoluciones = true;
                        else if (nodo.Content.ToString() == "Generar reporte de ventas")
                            vm.ReporteDeVentas = true;
                        else if (nodo.Content.ToString() == "Generar reporte de mas vendidos")
                            vm.ReporteMasVendidos = true;

                    }
                }
                
            }
          await  vm.NuevoUsuarioAsync();

        }
    }
}
