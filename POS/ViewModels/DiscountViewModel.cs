
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

namespace POS.ViewModels
{
    public class DiscountViewModel : BaseViewModel
    {
        private HttpClient httpClient;
        public const string url = "http://localhost:9095/api/";
        public Dictionary<Usuario, Seleccionado> seleccionados;

        public string nombre;
        public double porcentaje;
        public List<Descuento> descuentos;

        public Dictionary<Descuento, Dictionary<Usuario, bool>> cambioSeleccion;
        public string Nombre
        {
            get { return nombre; }
            set { SetProperty(ref nombre, value); }
        }
        public List<DescuentosPermitidos> DescuentosPermitidos
        {
            get; set;
        }
        public double Porcentaje
        {
            get { return porcentaje; }
            set { SetProperty(ref porcentaje, value); }
        }
        public List<Descuento> Descuentos
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
            CambioSeleccion = new Dictionary<Descuento, Dictionary<Usuario, bool>>();
            Porcentaje = 0;
            DescuentosPermitidos = new List<DescuentosPermitidos>();
            httpClient = new HttpClient();
            res = httpClient.GetAsync(url + "DescuentosPermitidos/GetDescPermitidos/").Result;
            var cadena = res.Content.ReadAsStringAsync().Result;
            if (cadena.Length > 0)
            {
                DescuentosPermitidos = JsonSerializer.Deserialize<List<DescuentosPermitidos>>(res.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            foreach (var u in Usuarios)
            {
                Seleccionados.Add(u, new Seleccionado { Permitido = true });

            }



            NuevoDescuento = new RelayCommand(o => { AgregarDescuento(); });
            Descuentos = new List<Descuento>();
            httpClient = new HttpClient();
            HttpResponseMessage httpResponseMessage = httpClient.GetAsync(url + "Descuentos/GetDescuentos/").Result;
            Descuentos = JsonSerializer.Deserialize<List<Descuento>>(httpResponseMessage.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            foreach (var d in Descuentos)
            {
                var lista = new Dictionary<Usuario, bool>();
                foreach (var dp in DescuentosPermitidos)
                {
                    if (d.NoDescuento == dp.IdDescuento)
                    {
                        foreach (var u in Usuarios)
                        {
                            if (u.NoEmpleado == dp.NoEmpleado)
                                lista.Add(u, true);
                            else
                                lista.Add(u, false);
                        }
                    }
                }
                CambioSeleccion.Add(d, lista);
            }
            Quitar = new RelayCommand(QuitarUsuario);
        }
        private void AgregarDescuento()
        {
            var descuento = new Descuento();
            descuento.Nombre = Nombre;
            descuento.Porcentaje = Porcentaje;
            var json = JsonSerializer.Serialize(descuento);

            httpClient = new HttpClient();
            var content = new StringContent(json.ToString(), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage resp = httpClient.PostAsync(url + "Descuentos/AgregarDescuento/", content).Result;
            if (resp.IsSuccessStatusCode)
            {
                var id = JsonSerializer.Deserialize<Descuento>(resp.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                httpClient = new HttpClient();
                var newDicc = new Dictionary<Usuario, Seleccionado>();
                foreach (var s in Seleccionados)
                {
                    newDicc.Add(s.Key, s.Value);
                }

                json = JsonSerializer.Serialize(newDicc.ToList());
                MessageDialog v = new MessageDialog(json);
                content = new StringContent(json.ToString(), Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                resp = httpClient.PostAsync(url + "DescuentosPermitidos/AgregarDescPermitido/" + id.NoDescuento.ToString(), content).Result;

                if (resp.IsSuccessStatusCode)
                {
                    CambioSeleccion = new Dictionary<Descuento, Dictionary<Usuario, bool>>();
                    Descuentos.Add(descuento);
                    foreach (var d in Descuentos)
                    {
                        var lista = new Dictionary<Usuario, bool>();
                        foreach (var p in DescuentosPermitidos)
                        {

                            if (d.NoDescuento == p.IdDescuento)
                            {
                                var usuario = Usuarios.Where(b => b.NoEmpleado == p.NoEmpleado).First();
                                lista.Add(usuario, true);

                            }

                        }
                        CambioSeleccion.Add(d, lista);
                    }
                    var dialog = new MessageDialog("Descuento agregado exitosamente!", "Mensaje");
                    var mensaje = dialog.ShowAsync();
                    Nombre = "";
                    Porcentaje = 0;
                }


            }

        }
        private void QuitarUsuario(Object obj)
        {
            var usuario = obj as Usuario;
        }
    }
}
