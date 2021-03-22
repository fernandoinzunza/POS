using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using POS.Models;
using Windows.UI.Popups;

namespace POS.ViewModels
{
    public class ProviderViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private string nombre;
        private string razonSocial;
        private string rfc;
        private string contacto;
        private string telefono;
        private string correo;
        private HttpClient httpClient;
        private ObservableCollection<KeyValuePair<Proveedor, List<RelayCommand>>> proveedores;
        public const string url = "http://localhost:9095/api/Providers/";

        public string Correo
        {
            get { return correo; }
            set { SetProperty(ref correo, value); }
        }

        public string Telefono
        {
            get { return telefono; }
            set { SetProperty(ref telefono, value); }
        }

        public string Contacto
        {
            get { return contacto; }
            set { SetProperty(ref contacto, value); }
        }

        public string Rfc
        {
            get { return rfc; }
            set { SetProperty(ref rfc, value); }
        }

        public string RazonSocial
        {
            get { return razonSocial; }
            set { SetProperty(ref razonSocial, value); }
        }

        public string Nombre
        {
            get { return nombre; }
            set { SetProperty(ref nombre, value); }
        }
        public ObservableCollection<KeyValuePair<Proveedor, List<RelayCommand>>> Proveedores
        {
            get { return proveedores; }
            set { SetProperty(ref proveedores, value); }
        }
        public RelayCommand Eliminar { get; set; }
        public RelayCommand Editar { get; set; }
        public RelayCommand AgregarProveedor { get; set; }

        bool INotifyDataErrorInfo.HasErrors => throw new NotImplementedException();

        public ProviderViewModel()
        {
            httpClient = new HttpClient();
            HttpResponseMessage responseMessage = httpClient.GetAsync(url + "GetProviders/").Result;
            Proveedores = new ObservableCollection<KeyValuePair<Proveedor, List<RelayCommand>>>();
            var lista = JsonSerializer.Deserialize<ObservableCollection<Proveedor>>(responseMessage.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Eliminar = new RelayCommand(EliminarProveedor);
            Editar = new RelayCommand(EditarProveedor);
            var comandos = new List<RelayCommand>();
            comandos.Add(Eliminar);
            comandos.Add(Editar);
            foreach (var l in lista)
            {
                Proveedores.Add(new KeyValuePair<Proveedor, List<RelayCommand>>(l, comandos));
            }
            AgregarProveedor = new RelayCommand(async (o) => { await Agregar(); });

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

        public async Task Agregar()
        {
            var alerta = new MessageDialog("", "Mensaje del Sistema");
            if (Nombre == null)
            {
                alerta.Content = "Agrega el nombre del proveedor!";
                await alerta.ShowAsync();
            }
            else if (RazonSocial == null)
            {
                alerta.Content = "Agrega la razon social!";
                await alerta.ShowAsync();
            }
            else if (Nombre == null)
            {
                alerta.Content = "Agrega el RFC del proveedor!";
                await alerta.ShowAsync();
            }
            else if (Nombre == null)
            {
                alerta.Content = "Agrega el nombre de contacto del proveedor!";
                await alerta.ShowAsync();

            }
            else if (Nombre == null)
            {
                alerta.Content = "Agrega el correo del proveedor!";
                await alerta.ShowAsync();
            }
            else if (Telefono == null)
            {
                alerta.Content = "Agrega el telefono del proveedor!";
                await alerta.ShowAsync();
            }
            else
            {


                var proveedor = new Proveedor();
                proveedor.Nombre = Nombre;
                proveedor.RazonSocial = RazonSocial;
                proveedor.Rfc = Rfc;
                proveedor.Contacto = Contacto;
                proveedor.Correo = Correo;
                proveedor.Telefono = Telefono;
                var obj = JsonSerializer.Serialize(proveedor);
                var content = new StringContent(obj, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await httpClient.PostAsync("http://localhost:9095/api/Providers/AgregarProveedor", content);

                if (result.IsSuccessStatusCode)
                {
                    Nombre = "";
                    RazonSocial = "";
                    Rfc = "";
                    Contacto = "";
                    Correo = "";
                    Telefono = "";
                    var messageDialog = new MessageDialog("Proveedor agregado con exito!", "Mensaje del Sistema");
                    await messageDialog.ShowAsync();
                }
            }
        }
        private async void EliminarProveedor(object obj)
        {
            var proveedor = (KeyValuePair<Proveedor, List<RelayCommand>>)obj;
            var comandos = new List<RelayCommand>();
            comandos.Add(Eliminar);
            comandos.Add(Editar);
            KeyValuePair<Proveedor, List<RelayCommand>> keyValuePair = new KeyValuePair<Proveedor, List<RelayCommand>>(proveedor.Key, comandos);
            var prov = Proveedores.Where(b => b.Key.Id == keyValuePair.Key.Id).FirstOrDefault();
            Proveedores.Remove(prov);
            HttpResponseMessage response = await httpClient.DeleteAsync(url + proveedor.Key.Id);
            var md = new MessageDialog("", "Mensaje del Sistema");
            if (response.IsSuccessStatusCode)
            {
                md.Content = "Se elimino el proveedor correctamente";
                await md.ShowAsync();
            }
            else
            {
                md.Content = "Hubo un problema al eliminar al proveedor " + await response.Content.ReadAsStringAsync();
                await md.ShowAsync();
            }
        }
        private async void EditarProveedor(object obj)
        {
            var proveedor = (KeyValuePair<Proveedor, List<RelayCommand>>)obj;
            var comandos = new List<RelayCommand>();
            comandos.Add(Eliminar);
            comandos.Add(Editar);
            var json = JsonSerializer.Serialize(proveedor.Key);
            StringContent httpContent = new StringContent(json);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(url + "UpdateProveedor/", httpContent);
            var md = new MessageDialog("", "Mensaje del Sistema");
            if (response.IsSuccessStatusCode)
            {
                md.Content = "Se actualizo el proveedor correctamente";
                await md.ShowAsync();
            }
            else
            {
                md.Content = "Hubo un problema al actualizar al proveedor " + await response.Content.ReadAsStringAsync();
                await md.ShowAsync();
            }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
