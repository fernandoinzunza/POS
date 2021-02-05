using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using POS.Models;
namespace POS.ViewModels
{
    public class ArticleListViewModel : BaseViewModel
    {
        public const string url = "http://localhost:9095/api/Articulos/";
        private HttpClient httpClient;
        public string articleSelected;
        public ObservableCollection<Articulo> articulos;
        private string llave;
        public Articulo select;


        #region Properties

        public string Llave
        {
            get { return llave; }
            set { SetProperty(ref llave, value); }
        }
        public Articulo Select { get { return select; } set { SetProperty(ref select, value); } }
        public RelayCommand Editar { get; set; }
        public RelayCommand Eliminar { get => new RelayCommand(EliminarFunction); }
        public ObservableCollection<Articulo> Articulos { get { return articulos; } set { SetProperty(ref articulos, value); } }
        public RelayCommand Filtrar { get; set; }
        public string ArticleSelected
        {
            get { return articleSelected; }
            set { SetProperty(ref articleSelected, value); }
        }
        #endregion
        public ArticleListViewModel()
        {
            httpClient = new HttpClient();
            HttpResponseMessage responseMessage = httpClient.GetAsync(url + "GetArticulos/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var lista = responseMessage.Content.ReadAsStringAsync().Result;
                Articulos = JsonSerializer.Deserialize<ObservableCollection<Articulo>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            Editar = new RelayCommand(EditarFunction);
        }
        #region Commands


        private void EliminarFunction(Object obj)
        {
            var articulo = obj as Articulo;
            var dialog = new MessageDialog("El articulo con clave " + articulo.Clave + " se eliminara", "Advertencia");
            var objeto = JsonSerializer.Serialize(articulo);
            var contenido = new StringContent(objeto, Encoding.UTF8);
            contenido.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            dialog.Commands.Add(new UICommand("Aceptar", delegate (IUICommand command)
             {
                 httpClient = new HttpClient();
                 HttpResponseMessage responseMessage = httpClient.PostAsync(url + "EliminarArticulo/", contenido).Result;
                 if (responseMessage.IsSuccessStatusCode)
                 {
                     var aux = responseMessage.Content.ReadAsStringAsync().Result;
                     var o = JsonSerializer.Deserialize<Articulo>(aux.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                     var artAQuitar = Articulos.Single(b => b.Clave == o.Clave);
                     if (Articulos.Remove(artAQuitar))
                     {
                         var content = new ToastContentBuilder()
                                      .AddToastActivationInfo("picOfHappyCanyon", ToastActivationType.Foreground)
                                      .AddText("Articulo eliminado")
                                      .AddText("El articulo con clave " + articulo.Clave + " fue eliminado correctamente")
                                      .GetToastContent();
                         var notif = new ToastNotification(content.GetXml());
                         ToastNotificationManager.CreateToastNotifier().Show(notif);
                     }
                 }




                // And show it!

            }));
            dialog.Commands.Add(new UICommand("No")
            {

            });
            // Show dialog and save result
            var result = dialog.ShowAsync();
        }

        private void EditarFunction(Object obj)
        {

            var row = obj as Articulo;
            var dialog = new MessageDialog("Seguro que quieres editar este articulo?", "Mensaje");
            var objeto = JsonSerializer.Serialize(row);
            var contenido = new StringContent(objeto, Encoding.UTF8);
            contenido.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            // If you want to add custom buttons

            dialog.Commands.Add(new UICommand("Aceptar", delegate (IUICommand command)
           {

               
               httpClient = new HttpClient();
               HttpResponseMessage responseMessage = httpClient.PostAsync(url + "UpdateArticulo/", contenido).Result;

               if (responseMessage.IsSuccessStatusCode)
               {

                   var content = new ToastContentBuilder()
                                  .AddToastActivationInfo("picOfHappyCanyon", ToastActivationType.Foreground)
                                  .AddText("Articulo editado")
                                  .AddText("El articulo con clave " + row.Clave + " fue editado correctamente")
                                  .GetToastContent();
                   var notif = new ToastNotification(content.GetXml());

                    // And show it!
                    ToastNotificationManager.CreateToastNotifier().Show(notif);
                   HttpResponseMessage response = httpClient.GetAsync(url + "GetArticulos/").Result;
                   if (response.IsSuccessStatusCode)
                   {
                       var lista = response.Content.ReadAsStringAsync().Result;
                       Articulos = JsonSerializer.Deserialize<ObservableCollection<Articulo>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                   }
               }


           }));
            dialog.Commands.Add(new UICommand("No")
            {

            });
            // Show dialog and save result
            var result = dialog.ShowAsync();


        }
        #endregion

    }
}
