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
        }

        private async Task Logear()
        {
            
            var user = new Usuario { NomUsuario = Usuario, Contrasena = Contrasena };
            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await cliente.PostAsync("http://localhost:9095/api/Usuarios/Login/",content);
            var res = await response.Content.ReadAsStringAsync();
            if(Convert.ToBoolean(res))
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage re = await cliente.PostAsync("http://localhost:9095/api/Usuarios/GetUsuario/", content);
                if(re.IsSuccessStatusCode)
                {
                    var datos = JsonSerializer.Deserialize<Usuario>(await re.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    localSettings.Values["NoEmpleado"] = datos.NoEmpleado;
                    localSettings.Values["NomUsuario"] = datos.NomUsuario;
                    localSettings.Values["ApPat"] = datos.ApPaterno;
                    localSettings.Values["ApMat"] = datos.ApMaterno;
                    localSettings.Values["Nombre"] = datos.Nombre;
                }

                navigationService.Navigate(typeof(StartPage));
            }

        }
    }
}
