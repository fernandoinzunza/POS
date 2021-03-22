using Microsoft.EntityFrameworkCore;
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
using POS.Models;
using System.Net.Http;
// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace POS.Views
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    ///
   
    public sealed partial class ArticleList : Page
    {
        public List<Descuento> ListaDescuentos { get; set; }
        HttpClient httpClient;
        ArticleListViewModel vm;
        public ArticleList()
        {
            this.InitializeComponent();
            ListaDescuentos = new List<Descuento>();
            vm = new ArticleListViewModel();
            this.DataContext = vm;
            ListaDescuentos = vm.GetDescuentos();
        }



        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text == "")
            {
                vm.Articulos = new System.Collections.ObjectModel.ObservableCollection<Articulo>(vm.AuxArticulos);
            }
            else
            {
                vm.Articulos = new System.Collections.ObjectModel.ObservableCollection<Articulo>(vm.Articulos.Where(b => b.Clave.Contains(textBox.Text) || b.Descripcion.ToUpper().Contains(textBox.Text.ToUpper())).ToList());
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(((ArticleListViewModel)(DataContext)).Articulos.Count > 0)
            {
                foreach (var item in e.AddedItems)
                {
                    ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                    lvi.ContentTemplate = (DataTemplate)this.Resources["Detail"];
                }
                //Remove DataTemplate for unselected items
                foreach (var item in e.RemovedItems)
                {
                    ListViewItem lvi = (sender as ListView).ContainerFromItem(item) as ListViewItem;
                    lvi.ContentTemplate = (DataTemplate)this.Resources["Normal"];
                }
            }
            
        }

        private void lista_ItemClick(object sender, ItemClickEventArgs e)
        {

            ListViewItem lvi = (sender as ListView).ContainerFromItem(e.ClickedItem) as ListViewItem;
            if(lvi.ContentTemplate == (DataTemplate)this.Resources["Detail"])
                lvi.ContentTemplate = (DataTemplate)this.Resources["Normal"];
            else
                lvi.ContentTemplate = (DataTemplate)this.Resources["Detail"];
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var articulo = btn.DataContext as Articulo;
            vm.EliminarFunction(articulo);
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var articulo = btn.DataContext as Articulo;
            vm.EditarFunction(articulo);
        }
    }
}
