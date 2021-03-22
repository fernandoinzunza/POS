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
using ClosedXML.Excel;
using Windows.Storage;
using System.Collections;

namespace POS.ViewModels
{
    public class ArticleListViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        public const string url = "http://localhost:9095/api/Articulos/";
        public const string urlDesc = "http://localhost:9095/api/Descuentos/";
        private HttpClient httpClient;
        public string articleSelected;
        public ObservableCollection<Articulo> articulos;

        private string llave;
        public Articulo select;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;


        #region Properties
        public ObservableCollection<Articulo> AuxArticulos { get; set; }
        public string Llave
        {
            get { return llave; }
            set { SetProperty(ref llave, value); }
        }
        public Articulo Select { get { return select; } set { SetProperty(ref select, value); } }
        public RelayCommand Editar { get; set; }
        public RelayCommand Eliminar { get => new RelayCommand(EliminarFunction); }
        public RelayCommand ArticulosAExcel { get; set; }
        public ObservableCollection<Articulo> Articulos { get { return articulos; } set { SetProperty(ref articulos, value); } }
        public RelayCommand Filtrar { get; set; }
        public string ArticleSelected
        {
            get { return articleSelected; }
            set { SetProperty(ref articleSelected, value); }
        }

        bool INotifyDataErrorInfo.HasErrors => throw new NotImplementedException();
        #endregion
        public ArticleListViewModel()
        {
            httpClient = new HttpClient();
            Articulos = new ObservableCollection<Articulo>();
            AuxArticulos = new ObservableCollection<Articulo>();
            HttpResponseMessage responseMessage = httpClient.GetAsync(url + "GetArticulos/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var lista = responseMessage.Content.ReadAsStringAsync().Result;
                Articulos = JsonSerializer.Deserialize<ObservableCollection<Articulo>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                AuxArticulos = new ObservableCollection<Articulo>(Articulos);
            }
            Editar = new RelayCommand(EditarFunction);
            ArticulosAExcel = new RelayCommand(o => { ExportarExcel(); });
        }

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
        #region Commands
        public  List<Descuento> GetDescuentos()
        {
            HttpResponseMessage response =  httpClient.GetAsync(urlDesc + "GetDescuentos/").Result;
            var lista = JsonSerializer.Deserialize<List<Descuento>>( response.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return lista;
        }
        private void ExportarExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("LISTA DE ARTICULOS");
                var cont = 4;
                worksheet.Cell("B1").Value = "Lista de articulos";
                worksheet.Cell("A3").Value = "Clave del articulo";
                worksheet.Cell("B3").Value = "Descripcion";
                worksheet.Cell("C3").Value = "Departamento";
                worksheet.Cell("D3").Value = "Precio al publico";
                worksheet.Cell("E3").Value = "Precio por mayoreo";
                worksheet.Cell("F3").Value = "Unidad";
                foreach (var art in Articulos)
                {
                    worksheet.Cell("B" + cont.ToString()).Value = art.Clave;
                    worksheet.Cell("C" + cont.ToString()).Value = art.Descripcion;
                    worksheet.Cell("D" + cont.ToString()).Value = art.Departamento;
                    worksheet.Cell("E" + cont.ToString()).Value = art.PrecioPublico;
                    worksheet.Cell("F" + cont.ToString()).Value = art.PrecioMayoreo;
                    worksheet.Cell("G" + cont.ToString()).Value = art.Unidad;
                    cont++;
                }
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 30;
                worksheet.Column(7).Width = 30;

                worksheet.Style.Font.SetFontName("Arial");
                worksheet.Style.Font.SetFontSize(12);
                worksheet.Style.Font.FontFamilyNumbering = XLFontFamilyNumberingValues.Roman;
                var range = worksheet.Range(worksheet.Cell("B3"), worksheet.LastCellUsed());

                range.CreateTable();
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                string dir = localFolder.Path + "\\Articulos.xlsx";
                try
                {
                    workbook.SaveAs(dir);
                    var md = new MessageDialog("Excel generado correctamente", "Mensaje del Sistema");
                    md.ShowAsync();
                }
                catch (Exception e)
                {
                    var md = new MessageDialog("Hubo un problema al generar el archivo " + e.Message, "Mensaje del Sistema");
                    md.ShowAsync();
                }

            }
        }

        public void EliminarFunction(Object obj)
        {

           bool tienePermiso = (bool)localSettings.Values["EliminarArticulo"];

            if (tienePermiso)
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
            else
            {
                var md = new MessageDialog("No tienes permisos para eliminar!!", "Mensaje del Sistema");
                md.ShowAsync();
            }
           
        }

        public void EditarFunction(Object obj)
        {
            bool tienePermiso = (bool)localSettings.Values["ModificarArticulo"];
            if(tienePermiso)
            {
                var row = obj as Articulo;
                var descuentos = GetDescuentos();
                var descSelected = descuentos.Where(b => b.NombreDescuento == row.NombreDescuento).First();
                row.IdDescuento = descSelected.NoDescuento;
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
                    }

                }));
                dialog.Commands.Add(new UICommand("No")
                {

                });
                // Show dialog and save result
                var result = dialog.ShowAsync();
            }
            else
            {
                var md = new MessageDialog("No tienes permiso para editar!!", "Mensaje del Sistema");
                md.ShowAsync();
            }
            


        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
