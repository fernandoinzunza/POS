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
    public sealed partial class CustomContentDialog : ContentDialog
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(CustomContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty FolioProperty = DependencyProperty.Register(
            "Folio", typeof(string), typeof(CustomContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ArticuloProperty = DependencyProperty.Register(
            "Articulo", typeof(string), typeof(CustomContentDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty CantidadProperty = DependencyProperty.Register(
            "Cantidad", typeof(double), typeof(CustomContentDialog), new PropertyMetadata(default(double)));
        public static readonly DependencyProperty TipoCancelacionProperty = DependencyProperty.Register(
            "TipoCancelacion", typeof(string), typeof(CustomContentDialog), new PropertyMetadata(default(string)));
        public CustomContentDialog()
        {
            this.InitializeComponent();
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public string TipoCancelacion
        {
            get { return (string)GetValue(TipoCancelacionProperty); }
            set { SetValue(TipoCancelacionProperty, value); }
        }
        public string Folio
        {
            get { return (string)GetValue(FolioProperty); }
            set { SetValue(FolioProperty, value); }
        }
        public string Articulo
        {
            get { return (string)GetValue(ArticuloProperty); }
            set { SetValue(ArticuloProperty, value); }
        }
        public double Cantidad
        {
            get { return (double)GetValue(CantidadProperty); }
            set { SetValue(CantidadProperty, value); }
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
