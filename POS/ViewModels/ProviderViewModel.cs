using System;
using System.Collections.Generic;
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
    public class ProviderViewModel : BaseViewModel
    {
        private string nombre;
        private string razonSocial;
        private string rfc;
        private string contacto;
        private string telefono;
        private string correo;
        private HttpClient httpClient;
        private List<Proveedor> proveedores;
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
        public List<Proveedor> Proveedores
        {
            get { return proveedores; }
            set { SetProperty(ref proveedores, value); }
        }
        public RelayCommand AgregarProveedor { get; set; }

        public ProviderViewModel()
        {
            httpClient = new HttpClient();
            HttpResponseMessage responseMessage = httpClient.GetAsync(url + "GetProviders/").Result;
            Proveedores = new List<Proveedor>();
            Proveedores = JsonSerializer.Deserialize<List<Proveedor>>(responseMessage.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            AgregarProveedor = new RelayCommand(async (o) => { await Agregar(); });
        }
        public async Task Agregar()
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
}
