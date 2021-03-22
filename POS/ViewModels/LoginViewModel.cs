using POS.Models;
using POS.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static System.Net.Mime.MediaTypeNames;

namespace POS.ViewModels
{
   public class LoginViewModel:BaseViewModel
    {
        private string usuario;
        private string contrasena;
        private string nivelUsuario;
        private HttpClient cliente;
        private const string urlPermisos = "http://localhost:9095/api/PermisosDisps/";
        private INavigationService navigationService;
        public string Contrasena
        {
            get { return contrasena; }
            set { SetProperty(ref contrasena, value); }
        }

        public string Usuario
        {
            get { return usuario; }
            set { SetProperty(ref usuario, value); }
        }

        public string NivelUsuario
        {
            get { return nivelUsuario; }
            set { SetProperty(ref nivelUsuario, value); }
        }
        public RelayCommand Login { get; set; }

        public LoginViewModel(INavigationService ns)
        {
            navigationService = ns;
            cliente = new HttpClient();
            Login = new RelayCommand(async (o) => { await Logear(); });
            IsBusy = false;
            TextVisible = true;

        }
        private bool textVisible;

        private bool isBusy;
        private bool incorrecto;
        public bool TextVisible
        {
            get { return textVisible; }
            set { SetProperty(ref textVisible, value); }
        }
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        public bool Incorrecto { get { return incorrecto; } set { SetProperty(ref incorrecto, value); } }
        private async Task Logear()
        {
            IsBusy = true;
            Incorrecto = false;
            TextVisible = false;
            var user = new Usuario { NomUsuario = Usuario, Contrasena = Contrasena };
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await cliente.PostAsync("http://localhost:9095/api/Usuarios/Login/", content);
            var res = await response.Content.ReadAsStringAsync();
            if(Convert.ToBoolean(res))
            {

                Incorrecto = false;
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage re =  await cliente.PostAsync("http://localhost:9095/api/Usuarios/GetUsuario/", content);
                if (re.IsSuccessStatusCode)
                {
                    var datos = JsonSerializer.Deserialize<Usuario>(await re.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    content = new StringContent(JsonSerializer.Serialize(datos));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await cliente.PostAsync("http://localhost:9095/api/PermisosDisps/" + "GetPermisosUsuario/",content);
                    if(response.IsSuccessStatusCode)
                    {
                        var permisos = JsonSerializer.Deserialize<Permisos>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        localSettings.Values["NoEmpleado"] = datos.NoEmpleado;
                        localSettings.Values["NomUsuario"] = datos.NomUsuario;
                        localSettings.Values["ApPat"] = datos.ApPaterno;
                        localSettings.Values["ApMat"] = datos.ApMaterno;
                        localSettings.Values["Nombre"] = datos.Nombre;
                        localSettings.Values["Puesto"] = datos.Puesto;

                        localSettings.Values["NuevoArticulo"] = permisos.NuevoArticulo;
                        localSettings.Values["ModificarArticulo"] = permisos.ModificarArticulo;
                        localSettings.Values["EliminarArticulo"] = permisos.EliminarArticulo;
                        localSettings.Values["NuevoDescuento"] = permisos.NuevoDescuento;
                        localSettings.Values["NuevaEntrada"] = permisos.NuevaEntrada;
                        localSettings.Values["EntradaPorReposicion"] = permisos.EntradaPorReposicion;
                        localSettings.Values["BitacoraEntradas"] = permisos.BitacoraEntradas;
                        localSettings.Values["NuevoProveedor"] = permisos.NuevoProveedor;
                        localSettings.Values["EditarProveedor"] = permisos.EditarProveedor;
                        localSettings.Values["EliminarProveedor"] = permisos.EliminarProveedor;
                        localSettings.Values["NuevaSalida"] = permisos.NuevaSalida;
                        localSettings.Values["BitacoraDeSalidas"] = permisos.BitacoraDeSalidas;
                        localSettings.Values["StockDisponible"] = permisos.StockDisponible;
                        localSettings.Values["VentanaDeCobro"] = permisos.VentanaDeCobro;
                        localSettings.Values["ReporteDeVentas"] = permisos.ReporteDeVentas;
                        localSettings.Values["RealizarCancelacion"] = permisos.RealizarCancelacion;
                        localSettings.Values["ReporteDeDevoluciones"] = permisos.ReporteDeDevoluciones;
                        localSettings.Values["ReporteMasVendidos"] = permisos.ReporteMasVendidos;



                    }
                   
                }
                IsBusy = false;
                TextVisible = true;
                navigationService.Navigate(typeof(StartPage));
            }
            else
            {
                IsBusy = false;
                Incorrecto = true;
                TextVisible = true;
            }

        }
    }
}
