using POS.Models;
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

// La plantilla de elemento del cuadro de diálogo de contenido está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS.CustomControls
{
    public sealed partial class DevolucionesContentDialog : ContentDialog
    {
        public static readonly DependencyProperty ListaDevolucionesProperty = DependencyProperty.Register(
            "ListaDevoluciones", typeof(ObservableCollection<Devolucion>), typeof(DevolucionesContentDialog), new PropertyMetadata(default(ObservableCollection<Devolucion>)));
        public DevolucionesContentDialog()
        {
            this.InitializeComponent();
        }
        public ObservableCollection<Devolucion> ListaDevoluciones
        {
            get { return (ObservableCollection<Devolucion>)GetValue(ListaDevolucionesProperty); }
            set { SetValue(ListaDevolucionesProperty, value); }
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
