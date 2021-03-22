using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace POS.ViewModels
{
    public class MeasuresViewModel : BaseViewModel
    {
        private string comboTipos;
        private HttpClient httpClient;
        private HttpResponseMessage responseMessage;
        private string nombre;
        private const string urlUnidades = "http://localhost:9095/api/UnidadMedidas/";
        private ObservableCollection<UnidadMedida> unidades;
        private Dictionary<UnidadMedida, List<string>> dicUnidades;
        public string ComboTipos { get { return comboTipos; } set { SetProperty(ref comboTipos, value); } }
        public string Nombre { get { return nombre; } set { SetProperty(ref nombre, value); } }
        public ObservableCollection<UnidadMedida> Unidades
        {
            get { return unidades; }
            set { SetProperty(ref unidades, value); }
        }
        public Dictionary<UnidadMedida,List<string>> DicUnidades
        {
            get { return dicUnidades; }
            set { SetProperty(ref dicUnidades, value); }
        }
        public RelayCommand AgregarMedida { get; set; }
        public RelayCommand EditarMedida { get; set; }
        public RelayCommand EliminarMedida { get; set; }

        public MeasuresViewModel()
        {
            Unidades = new ObservableCollection<UnidadMedida>();
            DicUnidades = new Dictionary<UnidadMedida, List<string>>();
            httpClient = new HttpClient();
            responseMessage = httpClient.GetAsync(urlUnidades + "GetUnidades/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var lista = responseMessage.Content.ReadAsStringAsync().Result;
                Unidades = JsonSerializer.Deserialize<ObservableCollection<UnidadMedida>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
           
            AgregarMedida = new RelayCommand(async (o) => { await NuevaMedida(); });
            EditarMedida = new RelayCommand(EditarAsync);
            

        }
        private async Task NuevaMedida()
        {
            var unidad = new UnidadMedida();
            unidad.Nombre = Nombre;
            unidad.TipoUnidad = ComboTipos;
            HttpClient httpClient = new HttpClient();
            var obj = JsonSerializer.Serialize(unidad);
            var content = new StringContent(obj, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await httpClient.PostAsync(urlUnidades + "AgregarUnidad/", content);
            if (result.IsSuccessStatusCode)
            {
                var mensaje = new MessageDialog("La unidad se agrego con exito", "Mensaje del Sistema");
                await mensaje.ShowAsync();
                Nombre = "";
            }
        }
        public async void EditarAsync(Object obj)
        {
            var medida = obj as UnidadMedida;
            var json = JsonSerializer.Serialize(medida);
            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            responseMessage = await httpClient.PostAsync(urlUnidades + "ActualizarMedida/", content);
            if(responseMessage.IsSuccessStatusCode)
            {
                Unidades.Remove(medida);
                var mens = new MessageDialog("Unidad actualizada correctamente!", "Mensaje del Sistema");
                await mens.ShowAsync();
            }
            else
            {
                if (await responseMessage.Content.ReadAsStringAsync() == "Esa unidad ya existe!")
                {
                    var mens = new MessageDialog("Esa unidad ya existe!", "Mensaje del Sistema");
                    await mens.ShowAsync();
                }
            }
        }
        public async void EliminarAsync(object obj)
        {
            var medida = obj as UnidadMedida;
            var json = JsonSerializer.Serialize(medida);
            var content = new StringContent(json.ToString(), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            responseMessage = await httpClient.PostAsync(urlUnidades + "EliminarMedida/", content);
            if(responseMessage.IsSuccessStatusCode)
            {
                Unidades.Remove(medida);
                var mens = new MessageDialog("Se elimino la unidad correctamente", "Mensaje del sistema");
                await mens.ShowAsync();
            }
            else
            {
                var mens = new MessageDialog("Hubo un problema al eliminar la unidad. "+ await responseMessage.Content.ReadAsStringAsync(), "Mensaje del sistema");
                await mens.ShowAsync();
            }
        }
    }
}
