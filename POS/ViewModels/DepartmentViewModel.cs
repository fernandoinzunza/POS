
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using POS.Models;
namespace POS.ViewModels
{
    public class DepartmentViewModel : BaseViewModel
    {
        private List<Departamento> departamentos;
        private object selectedRow;
        private HttpClient _httpClient;
        private Dictionary<string,List<Articulo>> articulosDep;
        public List<Departamento> Departamentos
        {
            get { return departamentos; }
            set { SetProperty(ref departamentos, value); }
        }

        public Dictionary<string, List<Articulo>> ArticulosDep
        {
            get { return articulosDep; }
            set { SetProperty(ref articulosDep, value); }
        }

        public object SelectedRow
        {
            get { return selectedRow; }
            set { SetProperty(ref selectedRow, value);
                var dep = SelectedRow as Departamento;
            }
        }
        public DepartmentViewModel()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            HttpResponseMessage response = _httpClient.GetAsync("http://localhost:9095/api/Departamentos/GetDepartamentos/").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                var lista = response.Content.ReadAsStringAsync().Result;
                Departamentos = JsonSerializer.Deserialize<List<Departamento>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                //Make sure to add a reference to System.Net.Http.Formatting.dll
            }
            ArticulosDep = new Dictionary<string, List<Articulo>>();
            foreach( var d in Departamentos)
            {
                _httpClient = new HttpClient();
                
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var json = JsonSerializer.Serialize(d);
                var content = new StringContent(json.ToString(), Encoding.UTF8,"application/json");
                HttpResponseMessage resp = _httpClient.PostAsync("http://localhost:9095/api/Articulos/GetArticulosDep/",content).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

                var lista = JsonSerializer.Deserialize<List<Articulo>>(resp.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ArticulosDep.Add(d.Nombre, lista);
            }
        }
    }
}
