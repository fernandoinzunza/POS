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
    public sealed partial class UnitiesList : Page
    {
        MeasuresViewModel vm;
        List<string> list = new List<string>();
        
        public UnitiesList()
        {
            this.InitializeComponent();
            vm = new MeasuresViewModel();
            this.DataContext = vm;
            if(vm.DicUnidades.Count > 0)
                dataGrid.SelectedIndex = 0;
            list.Add("Unidad de peso");
            list.Add("Unidad de distancia");
            list.Add("Unidad por pieza");
            list.Add("Unidad de volumen");
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button boton = sender as Button;
            var medida = boton.DataContext as UnidadMedida;
            var viejo = vm.Unidades.Where(b => b.Id == medida.Id).First();
            viejo.TipoUnidad = medida.TipoUnidad;
            viejo.Nombre = medida.Nombre;
            vm.EditarAsync(medida);


            
            
            
            
          
        }

        private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var boton = sender as Button;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button boton = sender as Button;
            var medida = boton.DataContext as UnidadMedida;           
            vm.EliminarAsync(medida);
        }
    }
}
