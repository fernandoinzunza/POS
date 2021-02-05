using POS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace POS.ViewModels
{
    public class MeasuresViewModel:BaseViewModel
    {
        private string comboTipos;
        private string nombre;
        private const string urlUnidades = "http://localhost:9095/api/UnidadMedidas/";
        public string ComboTipos { get { return comboTipos; } set { SetProperty(ref comboTipos, value); } }
        public string Nombre { get { return nombre; } set { SetProperty(ref nombre, value); } }
        public RelayCommand AgregarMedida { get; set; }
        public MeasuresViewModel()
        {
            AgregarMedida = new RelayCommand(async (o) => { await NuevaMedida(); });
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
            var result = await httpClient.PostAsync(urlUnidades+"AgregarUnidad/", content);
            if(result.IsSuccessStatusCode)
            {
                var mensaje = new MessageDialog("La unidad se agrego con exito", "Mensaje del Sistema");
                await mensaje.ShowAsync();
                Nombre = "";
            }
        }
    }
}
