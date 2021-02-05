using Microsoft.Toolkit.Uwp.UI.Controls;
using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace POS.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        private List<Articulo> articulos;
        private HttpClient httpClient;
        private Dictionary<Articulo, bool> stocks;
        private const string urlArticulos = "http://localhost:9095/api/Articulos/";
        private const string urlDepartamentos = "http://localhost:9095/api/Departamentos/";
        private const string urlMoneda = "http://localhost:9095/api/Monedas/";
        private const string urlProveedores = "http://localhost:9095/api/Providers/";
        private const string urlEntradas = "http://localhost:9095/api/EntradaAlmacens/";
        private string LiveTime = DateTime.Now.ToLongDateString().ToString();
        private Dictionary<Articulo, string> colores;
        private List<Departamento> departamentos;
        private List<Articulo> articulosPordepartamento;
        private object comboDepartamento;
        private List<Moneda> monedas;
        private List<Proveedor> proveedores;
        private double costoTotal;
        private double cantidad;
        private double precioUnitario;
        private object comboArticulo;
        private object comboMoneda;
        private object comboProveedor;
        private string noFactura;
        private bool isFocused;
        private bool mismaFactura;
        private double totalCompra;
        private bool nueva;
        private bool porCaja;
        private double unidadXCajas;
        private double cantidadCajasCompradas;
        private ObservableCollection<EntradaAlmacen> listaTemporal;
        private Dictionary<EntradaAlmacen, RelayCommand> listaConComando;
        private bool porUnidad;
        private double precioXCaja;
        private Dictionary<EntradaAlmacen, List<EntradaAlmacen>> comprasAlmacen;
        private Dictionary<string, List<EntradaAlmacen>> entradasTemporal;
        public RelayCommand EntradaGrid { get => new RelayCommand(o => { NuevaEntrada(); }); }

        public Dictionary<EntradaAlmacen,RelayCommand> ListaConComando
        {
            get { return ListaConComando; }
            set { SetProperty(ref listaConComando, value); }
        }
        public Dictionary<string, List<EntradaAlmacen>> EntradasTemporal
        {
            get { return entradasTemporal; }
            set { SetProperty(ref entradasTemporal, value); }
        }
        
        public ObservableCollection<EntradaAlmacen> ListaTemporal
        {
            get
            {
                return listaTemporal;
            }
            set { SetProperty(ref listaTemporal, value); }

        }
        public double TotalCompra
        {
            get { return totalCompra; }
            set { SetProperty(ref totalCompra, value); }
        }
        public string DateValue { get; set; }
        public bool MismaFactura { get { return mismaFactura; } set { SetProperty(ref mismaFactura, value); } }

        public Dictionary<EntradaAlmacen, List<EntradaAlmacen>> ComprasAlmacen
        {
            get { return comprasAlmacen; }
            set { SetProperty(ref comprasAlmacen, value); }
        }
        public List<Articulo> Articulos
        {
            get { return articulos; }
            set { SetProperty(ref articulos, value); }
        }
        public List<Articulo> ArticulosPorDepartamento
        {
            get { return articulosPordepartamento; }
            set { SetProperty(ref articulosPordepartamento, value); }
        }
        public Dictionary<Articulo, string> Colores
        {
            get { return colores; }
            set { SetProperty(ref colores, value); }
        }
        public List<Departamento> Departamentos
        {
            get { return departamentos; }
            set { SetProperty(ref departamentos, value); }
        }
        public List<Moneda> Monedas
        {
            get { return monedas; }
            set { SetProperty(ref monedas, value); }
        }
        public List<Proveedor> Proveedores
        {
            get { return proveedores; }
            set { SetProperty(ref proveedores, value); }
        }
        public string NoFactura
        {
            get { return noFactura; }
            set { SetProperty(ref noFactura, value); }
        }
        public object ComboDepartamento
        {
            get { return comboDepartamento; }
            set
            {
                SetProperty(ref comboDepartamento, value);
                var combo = ComboDepartamento as Departamento;
                var id = combo.Id;
                ArticulosPorDepartamento = Articulos.Where(b => b.IdDepartamento == id).ToList();

            }
        }
        public object ComboArticulo
        {
            get { return comboArticulo; }
            set { SetProperty(ref comboArticulo, value); }
        }
        public object ComboProveedor
        {
            get { return comboProveedor; }
            set { SetProperty(ref comboProveedor, value); }
        }

        public object ComboMoneda
        {
            get { return comboMoneda; }
            set { SetProperty(ref comboMoneda, value); }
        }
        public double Cantidad
        {
            get { return cantidad; }
            set
            {
                SetProperty(ref cantidad, value);
                if (Cantidad != 0 && PrecioUnitario != 0)
                    CostoTotal = Cantidad * PrecioUnitario;
            }
        }
        public double PrecioXCaja
        {
            get { return precioXCaja; }
            set { SetProperty(ref precioXCaja, value);
            if(PorCaja)
                {
                    if(UnidadXCajas != 0 && CantidadCajasCompradas !=0 && PrecioXCaja !=0)
                    {
                        CostoTotal = CantidadCajasCompradas * PrecioXCaja;
                        PrecioUnitario = PrecioXCaja / UnidadXCajas;
                    }
                }
            }
        }
        public double UnidadXCajas
        {
            get { return unidadXCajas; }
            set { SetProperty(ref unidadXCajas, value);
                if(PorCaja)
                {
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0)
                        Cantidad = UnidadXCajas * CantidadCajasCompradas;
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0 && PrecioXCaja != 0)
                    {
                        CostoTotal = CantidadCajasCompradas * PrecioXCaja;
                        PrecioUnitario = PrecioXCaja / UnidadXCajas;
                    }
                }
            
            }
        }
        public double CantidadCajasCompradas
        {
            get { return cantidadCajasCompradas; }
            set { SetProperty(ref cantidadCajasCompradas, value);
                if (PorCaja)
                {
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0)
                        Cantidad = UnidadXCajas * CantidadCajasCompradas;
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0 && PrecioXCaja != 0)
                    {
                        CostoTotal = CantidadCajasCompradas * PrecioXCaja;
                        PrecioUnitario = PrecioXCaja / UnidadXCajas;
                    }
                }
            }
        }

        
        public double PrecioUnitario
        {
            get { return precioUnitario; }
            set
            {
                SetProperty(ref precioUnitario, value);
                if (Cantidad != 0 && PrecioUnitario != 0)
                    CostoTotal = Cantidad * PrecioUnitario;
            }
        }
        public bool IsFocused
        {
            get { return isFocused; }
            set { SetProperty(ref isFocused, value); }
        }
        public bool Nueva
        {
            get { return nueva; }
            set { SetProperty(ref nueva, value); }
        }
        public double CostoTotal
        {
            get { return costoTotal; }
            set { SetProperty(ref costoTotal, value); }
        }
        public bool PorCaja
        {
            get { return porCaja; }
            set
            {
                SetProperty(ref porCaja, value);
                PorUnidad = !PorCaja;
            }

        }
        public bool PorUnidad
        {
            get{ return porUnidad; }
            set { SetProperty(ref porUnidad, value);
            }
        }
        public RelayCommand AgregarEntrada { get; set; }
        public RelayCommand BorrarLista { get; set; }

        public Dictionary<Articulo, bool> Stocks { get { return stocks; } set { SetProperty(ref stocks, value); } }
        public InventoryViewModel()
        {

            MismaFactura = true;
            Nueva = false;
            TotalCompra = 0;
            PorCaja = true;
            PrecioXCaja = 0;
            PorUnidad = false;
            ListaTemporal = new ObservableCollection<EntradaAlmacen>();
            ListaConComando = new Dictionary<EntradaAlmacen, RelayCommand>();
            EntradasTemporal = new Dictionary<string, List<EntradaAlmacen>>();
            Articulos = new List<Articulo>();
            ArticulosPorDepartamento = new List<Articulo>();
            ComprasAlmacen = new Dictionary<EntradaAlmacen, List<EntradaAlmacen>>();
            Stocks = new Dictionary<Articulo, bool>();
            Colores = new Dictionary<Articulo, string>();
            httpClient = new HttpClient();
            Cantidad = 0.0;
            PrecioUnitario = 0.0;
            CostoTotal = 0.0;

            DateValue = DateTime.Now.ToString();
            //listaTemporal = new ObservableCollection<EntradaAlmacen>();
            //listaTemporal.CollectionChanged +=ContentListChanges;
            HttpResponseMessage responseMessage = httpClient.GetAsync(urlArticulos + "GetArticulos/").Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                var lista = responseMessage.Content.ReadAsStringAsync().Result;
                Articulos = JsonSerializer.Deserialize<List<Articulo>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            HttpResponseMessage mensaje = httpClient.GetAsync(urlDepartamentos + "GetDepartamentos/").Result;
            if (mensaje.IsSuccessStatusCode)
            {
                var lista = mensaje.Content.ReadAsStringAsync().Result;
                Departamentos = JsonSerializer.Deserialize<List<Departamento>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            mensaje = httpClient.GetAsync(urlMoneda + "GetMonedas/").Result;
            if (mensaje.IsSuccessStatusCode)
            {
                var lista = mensaje.Content.ReadAsStringAsync().Result;
                Monedas = JsonSerializer.Deserialize<List<Moneda>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            mensaje = httpClient.GetAsync(urlProveedores + "GetProviders/").Result;
            if (mensaje.IsSuccessStatusCode)
            {
                var lista = mensaje.Content.ReadAsStringAsync().Result;
                Proveedores = JsonSerializer.Deserialize<List<Proveedor>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            mensaje = httpClient.GetAsync(urlEntradas + "GetEntradasAlmacen/").Result;
            if (mensaje.IsSuccessStatusCode)
            {
                var lista = mensaje.Content.ReadAsStringAsync().Result;
                var aux = JsonSerializer.Deserialize<List<EntradaAlmacen>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var aux2 = aux.GroupBy(b => b.NoFactura).Select(g => g.FirstOrDefault()).ToList();
                foreach (var a in aux2)
                {
                    var listaPorFactura = aux.Where(b => b.NoFactura == a.NoFactura).ToList();
                    ComprasAlmacen.Add(a, listaPorFactura);
                }


            }
            foreach (var a in Articulos)
            {
                if (a.Stock <= a.StockBajo)
                {
                    Stocks.Add(a, true);
                }
                else
                {
                    Stocks.Add(a, false);
                }
            }
            foreach (var s in Stocks)
            {
                if (s.Value)
                    Colores.Add(s.Key, "Red");
                else
                    Colores.Add(s.Key, "White");
            }
            AgregarEntrada = new RelayCommand(async (o) => { await FinalizarEntradaAsync(); });
            BorrarLista = new RelayCommand(o =>
            {
                ListaTemporal = new ObservableCollection<EntradaAlmacen>();
                MismaFactura = true;
                IsFocused = true;
                TotalCompra = 0;
                PrecioUnitario = 0;
                CostoTotal = 0;
                Cantidad = 0;

            });
   
        }
        private void QuitarEntrada(EntradaAlmacen obj)
        {
            var entrada = obj as EntradaAlmacen;
            TotalCompra -=  entrada.CostoTotal;


        }
        private async Task FinalizarEntradaAsync()
        {
            if (ListaTemporal.Count > 0)
            {
                foreach (var entrada in ListaTemporal)
                {
                    var json = JsonSerializer.Serialize(entrada);
                    var content = new StringContent(json, Encoding.UTF8);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage responseMessage = await httpClient.PostAsync(urlEntradas + "AgregarEntrada/", content);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var articulo = Articulos.Where(b => b.Clave == entrada.ClaveArticulo).First();
                        var stock = articulo.Stock;
                        articulo.Stock = (float)Cantidad + stock;
                        articulo.PrecioAlCosto = PrecioUnitario;
                        articulo.IdProveedor = entrada.IdProveedor;
                        articulo.NombreProveedor = entrada.NombreProveedor;
                        if(PorCaja)
                        {
                            articulo.Caja = "Si";
                            articulo.CajasStock += (int)CantidadCajasCompradas;
                            articulo.UnidadesXCaja = UnidadXCajas;
                            articulo.Stock += (float)(UnidadXCajas * CantidadCajasCompradas);
                            articulo.PrecioXCaja = PrecioXCaja;

                        }
                        content = new StringContent(JsonSerializer.Serialize(articulo), Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        responseMessage = await httpClient.PostAsync(urlArticulos + "UpdateArticulo/", content);
                    }
                    else
                    {
                        var mensaje = new MessageDialog("Hubo un problema al registrar una de las entradas", "Mensaje del Sistema");
                        await mensaje.ShowAsync();
                        break;
                    }

                }
                var mens = new MessageDialog("Se registro la compra exitosamente!", "Mensaje del Sistema");
                await mens.ShowAsync();
                MismaFactura = true;
                IsFocused = true;
                Nueva = false;
            }
            else
            {
                var mens = new MessageDialog("No hay entradas a agregar", "Mensaje del Sistema");
                await mens.ShowAsync();
            }

        }
        /*
        var json = JsonSerializer.Serialize(entrada);
        var content = new StringContent(json, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpResponseMessage responseMessage = await httpClient.PostAsync(urlEntradas + "AgregarEntrada/", content);
        if (responseMessage.IsSuccessStatusCode)
        {
            var articulo = Articulos.Where(b => b.Clave == entrada.ClaveArticulo).First();
            var stock = articulo.Stock;
            articulo.Stock = (float)Cantidad + stock;
            articulo.PrecioAlCosto = PrecioUnitario;
            articulo.IdProveedor = entrada.IdProveedor;
            articulo.NombreProveedor = entrada.NombreProveedor;
            content = new StringContent(JsonSerializer.Serialize(articulo), Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            responseMessage = await httpClient.PostAsync(urlArticulos + "UpdateArticulo/", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                var mensaje = new MessageDialog("Se registro la entrada con exito, agregar otra entrada para la misma factura?", "Mensaje del Sistema");
                mensaje.Commands.Add(new UICommand("Misma factura", delegate (IUICommand command)
                {
                    PrecioUnitario = 0;
                    Cantidad = 0;
                    CostoTotal = 0;

                }));
                mensaje.Commands.Add(new UICommand("Nueva factura", delegate (IUICommand command)
                {
                    NoFactura = "";
                    PrecioUnitario = 0;
                    Cantidad = 0;
                    CostoTotal = 0;
                    IsFocused = true;

                }));
                await mensaje.ShowAsync();
            }

        */
        private void NuevaEntrada()
        {

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var entrada = new EntradaAlmacen();
            entrada.Articulo = (ComboArticulo as Articulo).Descripcion;
            entrada.Cantidad = Cantidad;
            entrada.ClaveArticulo = (ComboArticulo as Articulo).Clave;
            entrada.CostoTotal = CostoTotal;
            entrada.Departamento = (ComboDepartamento as Departamento).Nombre;
            entrada.NombreMoneda = (ComboMoneda as Moneda).Nombre;
            entrada.IdMoneda = (ComboMoneda as Moneda).Id;
            entrada.IdProveedor = (ComboProveedor as Proveedor).Id;
            entrada.NombreProveedor = (ComboProveedor as Proveedor).Nombre;
            entrada.PrecioUnitario = PrecioUnitario;
            entrada.IdDepartamento = (ComboDepartamento as Departamento).Id;
           // entrada.CajasStock = (int)CantidadCajasCompradas;
            //entrada.UnidadesXCaja = UnidadXCajas;
            entrada.Fecha = DateValue;
            entrada.Caja = PorCaja ? "Si" :"No";
            entrada.NoFactura = NoFactura;
            entrada.NoEmpleado = (int)localSettings.Values["NoEmpleado"];
            entrada.NombreEmpleado = (string)localSettings.Values["Nombre"] + " " + (string)localSettings.Values["ApPat"] + " " + (string)localSettings.Values["ApMat"];
            TotalCompra += CostoTotal;
            listaTemporal.Add(entrada);
            MismaFactura = false;
            Nueva = true;
            /*
            var json = JsonSerializer.Serialize(entrada);
            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage responseMessage = await httpClient.PostAsync(urlEntradas + "AgregarEntrada/",content);
            if(responseMessage.IsSuccessStatusCode)
            {
                var articulo = Articulos.Where(b => b.Clave == entrada.ClaveArticulo).First();
                var stock = articulo.Stock;
                articulo.Stock = (float)Cantidad + stock;
                articulo.PrecioAlCosto = PrecioUnitario;
                articulo.IdProveedor = entrada.IdProveedor;
                articulo.NombreProveedor = entrada.NombreProveedor;
                content = new StringContent(JsonSerializer.Serialize(articulo), Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                responseMessage = await httpClient.PostAsync(urlArticulos + "UpdateArticulo/", content);
                if(responseMessage.IsSuccessStatusCode)
                {
                    var mensaje = new MessageDialog("Se registro la entrada con exito, agregar otra entrada para la misma factura?", "Mensaje del Sistema");
                    mensaje.Commands.Add(new UICommand("Misma factura", delegate (IUICommand command)
                    {
                        PrecioUnitario = 0;
                        Cantidad = 0;
                        CostoTotal = 0;                      

                    }));
                    mensaje.Commands.Add(new UICommand("Nueva factura", delegate (IUICommand command)
                    {
                        NoFactura = "";
                        PrecioUnitario = 0;
                        Cantidad = 0;
                        CostoTotal = 0;
                        IsFocused = true;

                    }));
                    await mensaje.ShowAsync();
                }


            }
            */

        }
    }

}
