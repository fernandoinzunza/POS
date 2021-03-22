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
    public sealed partial class Devoluciones : Page
    {
        SellViewModel vm;
        
        public Devoluciones()
        {
            this.InitializeComponent();
            vm = new SellViewModel();
            this.DataContext = vm;
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
           
            var txt = sender as TextBox;
            if(txt.Text == "")
            {
                vm.ListaDevoluciones = vm.AuxDevoluciones;
            }
            else
            {
                var aux = vm.AuxDevoluciones;
                vm.ListaDevoluciones = new System.Collections.ObjectModel.ObservableCollection<Models.Devolucion>(aux.Where(b => b.Folio.ToString().Contains(txt.Text)).ToList());
            }
            
        }
    }
}
