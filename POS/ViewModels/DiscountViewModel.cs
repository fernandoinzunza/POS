
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Popups;
using POS.Models;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace POS.ViewModels
{
    public class DiscountViewModel : BaseViewModel
    {
        private HttpClient httpClient;
        public const string url = "http://localhost:9095/api/";
        public Dictionary<Usuario, Seleccionado> seleccionados;

        public string nombre;
        public double porcentaje;
        public ObservableCollection<Descuento> descuentos;

        public Dictionary<Descuento, Dictionary<Usuario, bool>> cambioSeleccion;
        public string Nombre
        {
            get { return nombre; }
            set { SetProperty(ref nombre, value); }
        }
        public ObservableCollection<Descuento> AuxDescuentos
        {
            get; set;
        }

        public double Porcentaje
        {
            get { return porcentaje; }
            set { SetProperty(ref porcentaje, value); }
        }
        public ObservableCollection<Descuento> Descuentos
        {
            get { return descuentos; }
            set { SetProperty(ref descuentos, value); }
        }
        public Dictionary<Descuento, Dictionary<Usuario, bool>> CambioSeleccion
        {
            get { return cambioSeleccion; }
            set { SetProperty(ref cambioSeleccion, value); }
        }
        public RelayCommand NuevoDescuento { get; set; }
        public RelayCommand Quitar { get; set; }
        public RelayCommand BuscarDescuento { get; set; }
        public Dictionary<Usuario, Seleccionado> Seleccionados
        {
            get { return seleccionados; }
            set { SetProperty(ref seleccionados, value); }
        }
        public List<Usuario> Usuarios
        { get; set; }
        public DiscountViewModel()
        {
            httpClient = new HttpClient();
            Usuarios = new List<Usuario>();
            HttpResponseMessage res = httpClient.GetAsync(url + "Usuarios/GetUsuarios/").Result;
            Usuarios = JsonSerializer.Deserialize<List<Usuario>>(res.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Seleccionados = new Dictionary<Usuario, Seleccionado>();
            AuxDescuentos = new ObservableCollection<Descuento>();
            CambioSeleccion = new Dictionary<Descuento, Dictionary<Usuario, bool>>();
            Porcentaje = 0;
            httpClient = new HttpClient();

            foreach (var u in Usuarios)
            {
                Seleccionados.Add(u, new Seleccionado { Permitido = true });

            }



            NuevoDescuento = new RelayCommand(o => { AgregarDescuento(); });
            Descuentos = new ObservableCollection<Descuento>();
            httpClient = new HttpClient();
            HttpResponseMessage httpResponseMessage = httpClient.GetAsync(url + "Descuentos/GetDescuentos/").Result;
            Descuentos = new ObservableCollection<Descuento>(JsonSerializer.Deserialize<List<Descuento>>(httpResponseMessage.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
            AuxDescuentos = new ObservableCollection<Descuento>(Descuentos);
            Quitar = new RelayCommand(QuitarUsuario);
            BuscarDescuento = new RelayCommand(BuscarDesc);
        }
        private  void BuscarDesc(object obj)
        {
            var sender = obj as Windows.UI.Xaml.Input.KeyRoutedEventArgs;
            var textbox = sender.OriginalSource as TextBox;
            if (textbox.Text == "")
            {
                Descuentos = new ObservableCollection<Descuento>(AuxDescuentos);
            }
            else
            {
                Descuentos = new ObservableCollection<Descuento>(Descuentos.Where(b => b.NombreDescuento.ToUpper().Contains(textbox.Text.ToUpper())));
            }
        }
        private async void AgregarDescuento()
        {
            var descuento = new Descuento();
            descuento.NombreDescuento = Nombre;
            descuento.Porcentaje = Porcentaje;
            var json = JsonSerializer.Serialize(descuento);

            httpClient = new HttpClient();
            var content = new StringContent(json.ToString(), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage resp = httpClient.PostAsync(url + "Descuentos/AgregarDescuento/", content).Result;
            if (resp.IsSuccessStatusCode)
            {
                var md = new MessageDialog("Se agrego el descuento correctamente", "Mensaje del sistema");
                await md.ShowAsync();
                  
            }

        }
        private void QuitarUsuario(Object obj)
        {
            var usuario = obj as Usuario;
        }
    }
}
