using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using POS.Models;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;

namespace POS.ViewModels
{
    public class NewArticleViewModel : BaseViewModel
    {
        private bool checkBox1;
        private bool checkBox2;
        private bool checkBox3;
        private bool checkBox4;
        private bool newSection;
        private bool oldSection;
        private string txtClave;
        private string txtDescripcion;
        private string txtDepartamento;
        private double txtPrecioPublico;
        private double txtPrecioSecundario;
        private double txtPrecioAlCosto;
        private double txtPrecioMayoreo;
        private double txtStock;
        private string txtTipoUnidad;
        private object comboUnidad;
        private bool radioPeso;
        private bool radioDistancia;
        private List<Departamento> departamentos;
        private object comboDep;
        private bool isBusy;
        private List<Proveedor> proveedores;
        private object comboProveedores;
        private string _baseUrl = "https://localhost:5001";
        private double stockBajo;
        private HttpClient _httpClient;
        private string comboTipos;
        private List<UnidadMedida> unidadesPorTipo;

        public RelayCommand DistanciaChecked { get; set; }
        public RelayCommand PesoChecked { get; set; }
        public RelayCommand CheckClick { get; set; }
        public RelayCommand Check2Click { get; set; }
        public RelayCommand Check3Click { get; set; }
        public RelayCommand Check4Click { get; set; }
        public RelayCommand AddArticle { get; set; }
        public RelayCommand CheckOld { get; set; }
        public  NewArticleViewModel()
        {
            _httpClient = new HttpClient();
            CheckBox1 = false;
            CheckBox2 = false;
            CheckBox3 = false;
            CheckBox4 = false;
            IsBusy = false;
            CheckClick = new RelayCommand(o =>
            {
                Check1Cambio();
            });
            CheckOld = new RelayCommand(o => { CheckViejo(); });
            Check2Click = new RelayCommand(o => { Check2Cambio(); });
            Check3Click = new RelayCommand(o => { Check3Cambio(); });
            Check4Click = new RelayCommand(o => { Check4Cambio(); });
            AddArticle = new RelayCommand(async(o) => { await AgregarArticulo(); });
            PesoChecked = new RelayCommand(o => { RadioPesoF(); });
            DistanciaChecked = new RelayCommand(o => { RadioDistanciaF(); });
            Departamentos = new List<Departamento>();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           HttpResponseMessage response = _httpClient.GetAsync("http://localhost:9095/api/Departamentos/GetDepartamentos/").Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if(response.IsSuccessStatusCode)
            {
                var lista = response.Content.ReadAsStringAsync().Result;
                Departamentos = JsonSerializer.Deserialize<List<Departamento>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                //Make sure to add a reference to System.Net.Http.Formatting.dll

            }
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Proveedores = new List<Proveedor>();
            HttpResponseMessage r = _httpClient.GetAsync("http://localhost:9095/api/Providers/GetProviders").Result;
            if(r.IsSuccessStatusCode)
            {
                var lista = r.Content.ReadAsStringAsync().Result;
                Proveedores = JsonSerializer.Deserialize<List<Proveedor>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            r = _httpClient.GetAsync("http://localhost:9095/api/UnidadMedidas/GetUnidades").Result;
            if (r.IsSuccessStatusCode)
            {
                var lista = r.Content.ReadAsStringAsync().Result;
                Unidades = JsonSerializer.Deserialize<List<UnidadMedida>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            NewSection = true;
            OldSection = false;
        }
        #region Propiedades

        public object ComboProveedores { get { return comboProveedores; } set { SetProperty(ref comboProveedores, value); } }
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        public List<UnidadMedida> Unidades { get; set; }
        public object ComboDepartamentos
        {
            get { return comboDep; }
            set {
                SetProperty(ref comboDep, value); 
            
            }
        }
        public bool OldSection
        {
            get { return oldSection; }
            set
            {
                SetProperty(ref oldSection, value);
            }
        }
        public bool NewSection
        {
            get { return newSection; }
            set
            {
                SetProperty(ref newSection, value);

            }
        }

        public bool CheckBox1
        {
            get { return checkBox1; }
            set
            {
                this.SetProperty(ref this.checkBox1, value);
            }
        }
        public bool CheckBox2
        {
            get { return checkBox2; }
            set { SetProperty(ref checkBox2, value); }

        }

        public bool CheckBox3
        {
            get { return checkBox3; }
            set { SetProperty(ref checkBox3, value); }
        }
        public bool CheckBox4
        {
            get { return checkBox4; }
            set { SetProperty(ref checkBox4, value); }
        }
        public string TxtClave
        {
            get { return txtClave; }
            set { SetProperty(ref txtClave, value); }
        }
        public double TxtPrecioPublico
        {
            get { return txtPrecioPublico; }
            set { SetProperty(ref txtPrecioPublico, value); }
        }
        public double TxtPrecioSecundario
        {
            get { return txtPrecioSecundario; }
            set { SetProperty(ref txtPrecioSecundario, value); }
        }
        public double TxtPrecioAlCosto
        {
            get { return txtPrecioAlCosto; }
            set { SetProperty(ref txtPrecioAlCosto, value); }
        }


        public double TxtPrecioMayoreo
        {
            get { return txtPrecioMayoreo; }
            set { SetProperty(ref txtPrecioMayoreo, value); }
        }

        public string TxtDepartamento
        {
            get { return txtDepartamento; }
            set { SetProperty(ref txtDepartamento, value); }
        }


        public string TxtDescripcion
        {
            get { return txtDescripcion; }
            set { SetProperty(ref txtDescripcion, value); }
        }
        public double TxtStock
        {
            get { return txtStock; }
            set { SetProperty(ref txtStock, value); }
        }
        public string TxtTipoUnidad
        {
            get { return txtTipoUnidad; }
            set { SetProperty(ref txtTipoUnidad, value); }
        }
        public object ComboUnidad
        {
            get { return comboUnidad; }
            set { SetProperty(ref comboUnidad, value); }
        }
        public double StockBajo
        {
            get { return stockBajo; }
            set { SetProperty(ref stockBajo, value); }
        }
        #endregion

        public void RadioDistanciaF()
        {
            TxtTipoUnidad = "Distancia";
        }
        public void RadioPesoF()
        {
            TxtTipoUnidad = "Peso";
        }
        public bool RadioPeso
        {
            get { return radioPeso; }
            set { SetProperty(ref radioPeso, value); }
        }
        public bool RadioDistancia
        {
            get { return radioDistancia; }
            set { SetProperty(ref radioDistancia, value); }
        }
        public string ComboTipos
        {
            get { return comboTipos; }
            set { SetProperty(ref comboTipos, value);
            if(!string.IsNullOrEmpty(ComboTipos))
                {
                    UnidadesPorTipo = Unidades.Where(b => b.TipoUnidad == ComboTipos).ToList();
                }
            }
        }
        public List<UnidadMedida> UnidadesPorTipo
        {
            get { return unidadesPorTipo; }
            set { SetProperty(ref unidadesPorTipo,value); }
        }
        public List<Departamento> Departamentos
        {
            get { return departamentos; }
            set { SetProperty(ref departamentos, value); }
        }
        public List<Proveedor> Proveedores
        { get { return proveedores; } set { SetProperty(ref proveedores, value); } }

        public void Check1Cambio()
        {
            CheckBox1 = !CheckBox1;
            /*
            var dialog = new MessageDialog("Modulo en desarrollo", "Mensaje");

            // If you want to add custom buttons
            dialog.Commands.Add(new UICommand("Click me!", delegate (IUICommand command)
            {
                // Your command action here
            }));

            // Show dialog and save result
            var result = dialog.ShowAsync();
            */
        }
        public void Check2Cambio()
        {
            CheckBox2 = !CheckBox2;
        }
        public void Check3Cambio()
        {
            CheckBox3 = !CheckBox3;
        }
        public void Check4Cambio()
        {
            CheckBox4 = !CheckBox4;
        }
        public void CheckViejo()
        {
            NewSection = !NewSection;
            OldSection = !OldSection;
        }
        public async Task AgregarArticulo()
        {
            int id = 0;
            if (NewSection)
            {
                var departamento = new Departamento();
                departamento.Nombre = TxtDepartamento;
                HttpClient cliente = new HttpClient();
                var objeto = JsonSerializer.Serialize(departamento);
                var contenido = new StringContent(objeto, Encoding.UTF8);
                contenido.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var resultado = await cliente.PostAsync("http://localhost:9095/api/Departamentos/Agregar", contenido);
                var get = JsonSerializer.Deserialize<Departamento>( await resultado.Content.ReadAsStringAsync(),new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
                id = get.Id;
            
            }
            var articulo = new Articulo();
            articulo.Clave = TxtClave;
            if (NewSection)
                articulo.Departamento = TxtDepartamento;
            else
            {
                articulo.Departamento = (ComboDepartamentos as Departamento).Nombre;
                id = (ComboDepartamentos as Departamento).Id;
            }
            articulo.IdProveedor = null;
            articulo.NombreProveedor = null;
            articulo.IdDepartamento = id;
            articulo.Descripcion = TxtDescripcion;
            articulo.PrecioAlCosto = Convert.ToDouble(TxtPrecioAlCosto);
            articulo.PrecioMayoreo = Convert.ToDouble(TxtPrecioMayoreo);
            articulo.PrecioPublico = Convert.ToDouble(TxtPrecioPublico);
            articulo.PrecioSecundario = Convert.ToDouble(TxtPrecioSecundario);
            articulo.Stock = (float)TxtStock;
            articulo.TipoUnidad = ComboTipos;
            articulo.Unidad = (ComboUnidad as UnidadMedida).Nombre;
            articulo.StockBajo = StockBajo;
            articulo.Caja = "No";
            articulo.CajasStock = 0;
            articulo.UnidadesXCaja = 0;
            HttpClient httpClient = new HttpClient();
            var obj = JsonSerializer.Serialize(articulo);
            var content = new StringContent(obj, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var result = await httpClient.PostAsync("http://localhost:9095/api/Articulos/Agregar", content);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:9095/api/Departamentos/GetDepartamentos/");  // Blocking call! Program will wait here until a response is received or a timeout occurs.
            if (response.IsSuccessStatusCode)
            {
                var ob = response.Content.ReadAsStringAsync().Result;
                Departamentos = JsonSerializer.Deserialize<List<Departamento>>(ob);
                //Make sure to add a reference to System.Net.Http.Formatting.dll

            }
           
                TxtClave = "";
            TxtDepartamento = "";
            TxtDescripcion = "";
            TxtPrecioAlCosto = 0;
            TxtPrecioMayoreo = 0;
            TxtPrecioSecundario = 0;
            TxtPrecioPublico = 0;
            TxtStock = 0;

            TxtTipoUnidad = "";
            
            if(result.IsSuccessStatusCode)
            {
                var dialog = new MessageDialog("Articulo agregado exitosamente!", "Mensaje");

                // If you want to add custom buttons
                dialog.Commands.Add(new UICommand("Aceptar", delegate (IUICommand command)
                {
                    // Your command action here
                }));

                // Show dialog and save result
               await dialog.ShowAsync();
            }
            IsBusy = false;

        }


    }
}
