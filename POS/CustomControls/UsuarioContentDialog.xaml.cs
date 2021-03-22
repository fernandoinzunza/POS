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

// La plantilla de elemento del cuadro de diálogo de contenido está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS.CustomControls
{
    public sealed partial class UsuarioContentDialog : ContentDialog
    {
        public static readonly DependencyProperty NuevoArticuloProperty = DependencyProperty.Register(
            "NuevoArticulo", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ModificarArticuloProperty = DependencyProperty.Register(
            "ModificarArticulo", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty EliminarArticuloProperty = DependencyProperty.Register(
            "EliminarArticulo", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty NuevoDescuentoProperty = DependencyProperty.Register(
            "NuevoDescuento", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty NuevaEntradaProperty = DependencyProperty.Register(
            "NuevaEntrada", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty EntradaPorReposicionProperty = DependencyProperty.Register(
            "EntradaPorReposicion", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty BitacoraEntradasProperty = DependencyProperty.Register(
            "BitacoraEntradas", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty StockDisponibleProperty = DependencyProperty.Register(
            "StockDisponible", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty NuevaSalidaProperty = DependencyProperty.Register(
            "NuevaSalida", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty BitacoraDeSalidasProperty = DependencyProperty.Register(
            "BitacoraDeSalidas", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty NuevoProveedorProperty = DependencyProperty.Register(
            "NuevoProveedor", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty EliminarProveedorProperty = DependencyProperty.Register(
            "EliminarProveedor", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty EditarProveedorProperty = DependencyProperty.Register(
            "EditarProveedor", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty VentanaDeCobroProperty = DependencyProperty.Register(
            "VentanaDeCobro", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty RealizarCancelacionProperty = DependencyProperty.Register(
            "RealizarCancelacion", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ReporteDeDevolucionesProperty = DependencyProperty.Register(
            "ReporteDeDevoluciones", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ReporteDeVentasProperty = DependencyProperty.Register(
            "ReporteDeVentas", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ReporteMasVendidosProperty = DependencyProperty.Register(
            "ReporteMasVendidos", typeof(bool), typeof(UsuarioContentDialog), new PropertyMetadata(default(string)));
        public UsuarioContentDialog()
        {
            this.InitializeComponent();
        }
        public bool NuevoArticulo
        {
            get { return (bool)GetValue(NuevoArticuloProperty); }
            set { SetValue(NuevoArticuloProperty, value); }
        }
        public bool ModificarArticulo
        {
            get { return (bool)GetValue(ModificarArticuloProperty); }
            set { SetValue(ModificarArticuloProperty, value); }
        }
        public bool EliminarArticulo
        {
            get { return (bool)GetValue(EliminarArticuloProperty); }
            set { SetValue(EliminarArticuloProperty, value); }
        }
        public bool NuevoDescuento
        {
            get { return (bool)GetValue(NuevoDescuentoProperty); }
            set { SetValue(NuevoDescuentoProperty, value); }
        }
        public bool NuevaEntrada
        {
            get { return (bool)GetValue(NuevaEntradaProperty); }
            set { SetValue(NuevaEntradaProperty, value); }
        }
        public bool EntradaPorReposicion
        {
            get { return (bool)GetValue(EntradaPorReposicionProperty); }
            set { SetValue(EntradaPorReposicionProperty, value); }
        }
        public bool BitacoraDeSalidas
        {
            get { return (bool)GetValue(BitacoraDeSalidasProperty); }
            set { SetValue(BitacoraDeSalidasProperty, value); }
        }
        public bool BitacoraEntradas
        {
            get { return (bool)GetValue(BitacoraEntradasProperty); }
            set { SetValue(BitacoraEntradasProperty, value); }
        }
        public bool StockDisponible
        {
            get { return (bool)GetValue(StockDisponibleProperty); }
            set { SetValue(StockDisponibleProperty, value); }
        }
        public bool NuevaSalida
        {
            get { return (bool)GetValue(NuevaSalidaProperty); }
            set { SetValue(NuevaSalidaProperty, value); }
        }
        public bool NuevoProveedor
        {
            get { return (bool)GetValue(NuevoProveedorProperty); }
            set { SetValue(NuevoProveedorProperty, value); }
        }
        public bool EditarProveedor
        {
            get { return (bool)GetValue(EditarProveedorProperty); }
            set { SetValue(EditarProveedorProperty, value); }
        }
        public bool EliminarProveedor
        {
            get { return (bool)GetValue(EliminarProveedorProperty); }
            set { SetValue(EliminarProveedorProperty, value); }
        }
        public bool VentanaDeCobro
        {
            get { return (bool)GetValue(VentanaDeCobroProperty); }
            set { SetValue(VentanaDeCobroProperty, value); }
        }
        public bool RealizarCancelacion
        {
            get { return (bool)GetValue(RealizarCancelacionProperty); }
            set { SetValue(RealizarCancelacionProperty, value); }
        }
        public bool ReporteDeDevoluciones
        {
            get { return (bool)GetValue(ReporteDeDevolucionesProperty); }
            set { SetValue(ReporteDeDevolucionesProperty, value); }
        }
        public bool ReporteDeVentas
        {
            get { return (bool)GetValue(ReporteDeVentasProperty); }
            set { SetValue(ReporteDeVentasProperty, value); }
        }
        public bool ReporteMasVendidos
        {
            get { return (bool)GetValue(ReporteMasVendidosProperty); }
            set { SetValue(ReporteMasVendidosProperty, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
