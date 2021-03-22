using ClosedXML.Excel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using POS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Windows.Graphics.Printing;
using Windows.UI.Xaml.Printing;

namespace POS.ViewModels
{
    public class SellViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        protected PrintManager Printmgr;
        protected PrintDocument PrintDoc;


        private List<Articulo> articulos;
        private ObservableCollection<Carrito> compra;
        private HttpClient httpClient;
        private HttpResponseMessage httpResponseMessage;
        public const string urlArticulos = "http://localhost:9095/api/Articulos/";
        public const string urlCajas = "http://localhost:9095/api/CajaDispArticulos/";
        public const string urlVentas = "http://localhost:9095/api/Carritoes/";
        public const string urlSalidas = "http://localhost:9095/api/SalidasAlmacen/";
        public const string urlDepartamentos = "http://localhost:9095/api/Departamentos/";
        public const string urlDevoluciones = "http://localhost:9095/api/Devolucions/";
        public const string urlDescuentos = "http://localhost:9095/api/Descuentos/";
        private double totalCompra;
        private bool tarjeta;
        private bool efectivo;
        private DateTimeOffset desde;
        private DateTimeOffset hasta;
        private ObservableCollection<Devolucion> devoluciones;
        private ObservableCollection<Carrito> ventas;
        private ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>> ventasSeparadas;
        private ObservableCollection<EstadisticaArticulo> masVendidos;
        public List<Descuento> Descuentos { get; set; }
        private double totalVentaSinIva;
        private double totalVenta;
        public double TotalVentaSinIva
        {
            get { return totalVentaSinIva; }
            set { SetProperty(ref totalVentaSinIva, value); }
        }
        public ObservableCollection<Devolucion> AuxDevoluciones { get; set; }
        public ObservableCollection<EstadisticaArticulo> MasVendidos
        {
            get { return masVendidos; }
            set { SetProperty(ref masVendidos, value); }
        }
        public ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>> AuxVentasSeparadas { get; set; }
        public double TotalVenta
        {
            get { return totalVenta; }
            set { SetProperty(ref totalVenta, value); }
        }
        public DateTimeOffset Desde
        {
            get { return desde; }
            set
            {
                if (value > Hasta)
                {

                    DateTimeOffset outVar;
                    DateTimeOffset.TryParse("01/01/2021", out outVar);
                    Desde = outVar;
                    var md = new MessageDialog("La fecha final debe ser mayor a la fecha inicial", "Mensaje del Sistema");
                    ListaDevoluciones = new ObservableCollection<Devolucion>();
                    md.ShowAsync();
                }
                else
                {

                    SetProperty(ref desde, value);
                    List<Devolucion> aux = GetDevoluciones().Result;
                    if (Hasta != null && aux != null)
                        ListaDevoluciones = new ObservableCollection<Devolucion>(aux.Where(b => DateTimeOffset.Parse(b.FechaDevolucion) >= Desde && DateTimeOffset.Parse(b.FechaDevolucion) <= Hasta).ToList());


                    var listaAux = VentasSeparadas;
                    if (Hasta != null && aux != null && VentasSeparadas != null)
                    {
                        Total = 0;
                        TotalSinIva = 0;
                        var nuevaLista = AuxVentasSeparadas.Where(b => DateTimeOffset.Parse(b.Key.FechaVenta) >= Desde && DateTimeOffset.Parse(b.Key.FechaVenta) <= Hasta).Count();
                        if (nuevaLista > 0)
                        {
                            var lista = AuxVentasSeparadas.Where(b => DateTimeOffset.Parse(b.Key.FechaVenta) >= Desde && DateTimeOffset.Parse(b.Key.FechaVenta) <= Hasta).ToList();
                            VentasSeparadas = new ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>>(lista);
                            foreach (var v in VentasSeparadas)
                            {
                                Total += v.Key.SubTotal;
                                TotalSinIva += v.Key.SubTotal / 1.16;
                            }
                        }
                        else
                        {
                            VentasSeparadas = new ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>>();

                        }
                    }
                }

            }
        }
        private double total;
        private double totalSinIva;
        public double Total
        {
            get { return total; }
            set { SetProperty(ref total, value); }
        }
        public double TotalSinIva { get { return totalSinIva; } set { SetProperty(ref totalSinIva, value); } }
        public DateTimeOffset Hasta
        {
            get { return hasta; }
            set
            {
                if (value < Desde)
                {
                    var md = new MessageDialog("La fecha final debe ser mayor a la fecha inicial", " Mensaje del Sistema");
                    md.ShowAsync();
                    DateTimeOffset outVar;
                    DateTimeOffset.TryParse("01/01/2021", out outVar);
                    ListaDevoluciones = new ObservableCollection<Devolucion>();
                    Hasta = DateTime.Now;
                }
                else
                {
                    SetProperty(ref hasta, value);
                    List<Devolucion> aux = GetDevoluciones().Result;
                    if (Hasta != null && aux != null)
                        ListaDevoluciones = new ObservableCollection<Devolucion>(aux.Where(b => DateTimeOffset.Parse(b.FechaDevolucion) >= Desde && DateTimeOffset.Parse(b.FechaDevolucion) <= Hasta).ToList());
                    var listaAux = VentasSeparadas;

                    if (Hasta != null && aux != null && VentasSeparadas != null)
                    {
                        Total = 0;
                        TotalSinIva = 0;
                        var nuevaLista = AuxVentasSeparadas.Where(b => DateTimeOffset.Parse(b.Key.FechaVenta) >= Desde && DateTimeOffset.Parse(b.Key.FechaVenta) <= Hasta).Count();
                        if (nuevaLista > 0)
                        {
                            var lista = AuxVentasSeparadas.Where(b => DateTimeOffset.Parse(b.Key.FechaVenta) >= Desde && DateTimeOffset.Parse(b.Key.FechaVenta) <= Hasta).ToList();
                            VentasSeparadas = new ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>>(lista);
                            foreach (var v in VentasSeparadas)
                            {
                                Total += v.Key.SubTotal;
                                TotalSinIva += v.Key.SubTotal / 1.16;
                            }
                        }
                        else
                        {
                            VentasSeparadas = new ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>>();

                        }

                    }
                }
            }


        }



        public ObservableCollection<Devolucion> ListaDevoluciones
        {
            get { return devoluciones; }
            set { SetProperty(ref devoluciones, value); }
        }
        public List<Articulo> Articulos
        {
            get { return articulos; }
            set { SetProperty(ref articulos, value); }
        }
        public ObservableCollection<Carrito> Ventas
        {
            get { return ventas; }
            set { SetProperty(ref ventas, value); }
        }
        public ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>> VentasSeparadas
        {
            get { return ventasSeparadas; }
            set { SetProperty(ref ventasSeparadas, value); }
        }
        public List<CajaDispArticulo> Lista { get; set; }
        public List<Devolucion> Devoluciones { get; set; }
        public List<Departamento> Departamentos { get; set; }
        public double TotalCompra
        {
            get { return totalCompra; }
            set { SetProperty(ref totalCompra, value); }
        }
        public ObservableCollection<Carrito> Compra
        {
            get { return compra; }
            set { SetProperty(ref compra, value); }
        }
        public bool Tarjeta
        {
            get { return tarjeta; }
            set { SetProperty(ref tarjeta, value); }
        }
        public bool Efectivo
        {
            get { return efectivo; }
            set { SetProperty(ref efectivo, value); }
        }

        public List<Carrito> CompraPorUtilidad { get; set; }
        public RelayCommand FinalizarVenta { get; set; }
        public string DateValue { get; set; }
        public List<SalidaAlmacen> SalidasPorVenta { get; set; }
        public List<SalidaAlmacen> SalidasPorCancelacion { get; set; }
        public RelayCommand DevolucionesAExcel { get; set; }
        public RelayCommand VentasAExcel { get; set; }
        public RelayCommand MasVendidosAsc { get; set; }
        public RelayCommand MasVendidosDesc { get; set; }
        public RelayCommand Buscar { get; set; }
        public RelayCommand BuscarDevolucion { get; set; }

        public SellViewModel()
        {
            httpClient = new HttpClient();
            Efectivo = true;
            Articulos = new List<Articulo>();
            Compra = new ObservableCollection<Carrito>();
            SalidasPorVenta = new List<SalidaAlmacen>();
            SalidasPorCancelacion = new List<SalidaAlmacen>();
            httpResponseMessage = httpClient.GetAsync(urlArticulos + "GetArticulos").Result;
            Descuentos = new List<Descuento>();
            DateValue = DateTime.Now.ToString();
            Hasta = DateTimeOffset.Now.Date;
            Total = 0;
            TotalSinIva = 0;
            TotalVenta = 0;
            TotalVentaSinIva = 0;
            DateTimeOffset fecha;
            DateTimeOffset.TryParse("01/01/2021", out fecha);
            Desde = fecha.Date;
            var respuesta = httpClient.GetAsync(urlArticulos + "GetArticulos").Result;
            if (respuesta.IsSuccessStatusCode)
            {
                var lista = respuesta.Content.ReadAsStringAsync().Result;
                Articulos = JsonSerializer.Deserialize<List<Articulo>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            httpResponseMessage = httpClient.GetAsync(urlDepartamentos + "GetDepartamentos/").Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var json = httpResponseMessage.Content.ReadAsStringAsync().Result;
                Departamentos = JsonSerializer.Deserialize<List<Departamento>>(json.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            httpResponseMessage = httpClient.GetAsync(urlVentas + "GetCarritos/").Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var lista = httpResponseMessage.Content.ReadAsStringAsync().Result;
                Ventas = JsonSerializer.Deserialize<ObservableCollection<Carrito>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            httpResponseMessage = httpClient.GetAsync(urlDescuentos + "GetDescuentos/").Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var lista = httpResponseMessage.Content.ReadAsStringAsync().Result;
                Descuentos = JsonSerializer.Deserialize<List<Descuento>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            var distinct = Ventas.GroupBy(b => b.Folio).Select(g => g.FirstOrDefault()).ToList();
            VentasSeparadas = new ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>>();
            foreach (var d in distinct)
            {
                var nuevoCarrito = new Carrito();

                var collection = new ObservableCollection<Carrito>();
                double total = 0;
                double totalSinIva = 0;
                foreach (var v in Ventas)
                {

                    if (v.Folio == d.Folio && v.Cantidad != 0)
                    {
                        total += v.SubTotal;
                        totalSinIva += v.SubTotal / 1.16;
                        nuevoCarrito.SubTotal = total;
                        nuevoCarrito.SubTotalSinIva = totalSinIva;
                        nuevoCarrito.FechaVenta = v.FechaVenta;
                        collection.Add(v);
                    }


                }


                nuevoCarrito.Cancelado = d.Cancelado;
                nuevoCarrito.Cantidad = d.Cantidad;
                nuevoCarrito.ClaveArticulo = d.ClaveArticulo;
                nuevoCarrito.Folio = d.Folio;
                nuevoCarrito.RazonCancelado = d.RazonCancelado;
                nuevoCarrito.Cancelado = d.Cancelado;
                nuevoCarrito.Id = d.Id;
                nuevoCarrito.MetodoDePago = d.MetodoDePago;
                if (nuevoCarrito.Cantidad != 0)
                    VentasSeparadas.Add(new KeyValuePair<Carrito, ObservableCollection<Carrito>>(nuevoCarrito, collection));
            }
            foreach (var v in VentasSeparadas)
            {
                Total += v.Key.SubTotal;
                TotalSinIva += v.Key.SubTotal / 1.16;
            }
            AuxDevoluciones = new ObservableCollection<Devolucion>(GetDevoluciones().Result);
            AuxVentasSeparadas = new ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>>(VentasSeparadas);
            FinalizarVenta = new RelayCommand(async (o) => { await NuevaVentaAsync(); });
            CompraPorUtilidad = new List<Carrito>();
            ListaDevoluciones = new ObservableCollection<Devolucion>(GetDevoluciones().Result);
            DevolucionesAExcel = new RelayCommand(o => { ExportarAExcel(); });
            VentasAExcel = new RelayCommand(o => { ExportarVentasExcel(); });
            MasVendidos = new ObservableCollection<EstadisticaArticulo>();
            foreach (var art in Articulos)
            {
                double cantidad = 0;
                double totalVenta = 0;
                double totalVentaSinIva = 0;
                var clave = art.Clave;
                foreach (var venta in Ventas)
                {
                    if (venta.ClaveArticulo == clave)
                    {
                        cantidad += venta.Cantidad;
                        totalVenta += venta.SubTotal;
                        totalVentaSinIva += venta.SubTotal / 1.16;
                    }
                }
                var estadistica = new EstadisticaArticulo();
                estadistica.Cantidad = cantidad;
                estadistica.ClaveArticulo = clave;
                estadistica.DescripcionArticulo = art.Descripcion;
                estadistica.TotalConIva = totalVenta;
                estadistica.TotalSinIva = totalVentaSinIva;

                MasVendidos.Add(estadistica);
            }
            MasVendidos = new ObservableCollection<EstadisticaArticulo>(MasVendidos.OrderByDescending(b => b.Cantidad));
            MasVendidosAsc = new RelayCommand(o => { MasVendidos = new ObservableCollection<EstadisticaArticulo>(MasVendidos.OrderBy(b => b.Cantidad)); });
            MasVendidosDesc = new RelayCommand(o => { MasVendidos = new ObservableCollection<EstadisticaArticulo>(MasVendidos.OrderByDescending(b => b.Cantidad)); });
            Buscar = new RelayCommand(BuscarVenta);
            BuscarDevolucion = new RelayCommand(BuscarD);
        }

        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        private async void BuscarD(object obj)
        {
            var sender = obj as Windows.UI.Xaml.Input.KeyRoutedEventArgs;
            var textbox = sender.OriginalSource as TextBox;
            if (textbox.Text == "")
            {
                ListaDevoluciones = new ObservableCollection<Devolucion>(await GetDevoluciones());
            }
            else
            {
                ListaDevoluciones = new ObservableCollection<Devolucion>(ListaDevoluciones.Where(b => b.Folio.ToString().Contains(textbox.Text)));
            }
        }
        private void BuscarVenta(object obj)
        {
            var sender = obj as Windows.UI.Xaml.Input.KeyRoutedEventArgs;
            var textbox = sender.OriginalSource as TextBox;
            if (textbox.Text == "")
            {
                VentasSeparadas = AuxVentasSeparadas;
            }
            else
            {
                VentasSeparadas = new ObservableCollection<KeyValuePair<Carrito, ObservableCollection<Carrito>>>(VentasSeparadas.Where(b => b.Key.Folio.ToString().Contains(textbox.Text)).ToList<KeyValuePair<Carrito, ObservableCollection<Carrito>>>());
            }
        }
        public async Task<int> GetFolio()
        {
            httpResponseMessage = await httpClient.GetAsync(urlVentas + "GetVentas");
            var lista = JsonSerializer.Deserialize<List<Venta>>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return lista.Count;
        }

        private List<Microsoft.UI.Xaml.Controls.TabViewItem> tabViewItems;
        public List<Microsoft.UI.Xaml.Controls.TabViewItem> TabViewItems
        {
            get { return tabViewItems; }
            set { SetProperty(ref tabViewItems, value); }
        }
        public async Task<List<Devolucion>> GetDevoluciones()
        {
            httpResponseMessage = httpClient.GetAsync(urlDevoluciones + "GetDevoluciones/").Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var lista = JsonSerializer.Deserialize<List<Devolucion>>(httpResponseMessage.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var listaAux = new List<Devolucion>();
                foreach (var devolucion in lista)
                {
                    var folio = devolucion.Folio;
                    var carrito = devolucion.IdCarrito;
                    var existe = listaAux.Where(b => b.Folio == folio && b.IdCarrito == carrito).Count();
                    if (existe == 0)
                    {
                        listaAux.Add(devolucion);
                    }
                    else
                    {
                        var existente = listaAux.Where(b => b.Folio == folio && b.IdCarrito == carrito).First();
                        existente.Cantidad += devolucion.Cantidad;
                        existente.Perdida += devolucion.Perdida;
                        existente.PerdidaSivIva = existente.Perdida / 1.16;
                    }
                }


                return listaAux;
            }
            else
            {
                return new List<Devolucion>();
            }
        }
        public async Task ReponerMercancia(Carrito carrito, double cantidad, string razon, string tipoCancelacion)
        {
            CajasModificadasArticulos = new List<List<CajaDispArticulo>>();
            SalidasPorCancelacion = new List<SalidaAlmacen>();
            httpResponseMessage = await httpClient.GetAsync(urlCajas + "GetCajasDisp");
            var listaCajasDisp = JsonSerializer.Deserialize<List<CajaDispArticulo>>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var listaCajasPorArticulo = listaCajasDisp.Where(b => b.ClaveArticulo == carrito.ClaveArticulo).ToList();
            double stockTotal = 0;
            double utilidad = 0;
            foreach (var lpa in listaCajasPorArticulo)
            {
                stockTotal += lpa.StockTotal;
            }
            if (stockTotal >= cantidad)
            {
                foreach (var caja in listaCajasDisp)
                {
                    if (stockTotal >= cantidad)
                    {
                        var salidaPorReposicion = new SalidaAlmacen();
                        var devolucion = new Devolucion();
                        if (cantidad <= caja.StockTotal)
                        {
                            caja.StockTotal -= cantidad;

                            CompraPorUtilidad.Add(carrito);
                            if (cantidad <= caja.StockIndividual)
                            {
                                devolucion.IdCaja = caja.Id;
                                devolucion.IdCarrito = carrito.Id;
                                devolucion.Perdida = carrito.PrecioPublico * cantidad;
                                devolucion.PerdidaSivIva = devolucion.Perdida / 1.16;
                                devolucion.Razon = razon;
                                devolucion.TipoCancelacion = tipoCancelacion;
                                devolucion.Folio = carrito.Folio;
                                devolucion.FechaDevolucion = DateTime.Now.ToString();
                                devolucion.ClaveArticulo = carrito.ClaveArticulo;
                                devolucion.DescripcionArticulo = carrito.DescripcionArticulo;
                                devolucion.Cantidad = cantidad;
                                var contenido = new StringContent(JsonSerializer.Serialize(devolucion), Encoding.UTF8);
                                contenido.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                                httpResponseMessage = await httpClient.PostAsync(urlDevoluciones + "AgregarDevolucion/", contenido);
                                var idDevolucion = await httpResponseMessage.Content.ReadAsStringAsync();
                                caja.StockIndividual -= cantidad;
                                salidaPorReposicion.CajasSacadas = 0;
                                salidaPorReposicion.UnidadesSacadas = cantidad;
                                salidaPorReposicion.TotalPerdida = caja.PrecioUnitarioArt * cantidad;
                                salidaPorReposicion.ClaveArticulo = caja.ClaveArticulo;
                                salidaPorReposicion.IdDepartamento = Articulos.Where(b => b.Clave == caja.ClaveArticulo).First().Id;
                                salidaPorReposicion.NombreArticulo = carrito.DescripcionArticulo;
                                salidaPorReposicion.IdCaja = caja.Id;
                                salidaPorReposicion.IdDevolucion = JsonSerializer.Deserialize<Devolucion>(idDevolucion.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Id.ToString();
                                salidaPorReposicion.Razon = "Cancelacion parcial";
                                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                                salidaPorReposicion.NombreEmpleado = localSettings.Values["Nombre"] + " " + localSettings.Values["ApPat"] + " " + localSettings.Values["ApMat"];
                                salidaPorReposicion.FechaSalida = DateValue;
                                salidaPorReposicion.TotalPerdidaSinIva = salidaPorReposicion.TotalPerdida / 1.16;
                                SalidasPorCancelacion.Add(salidaPorReposicion);
                            }
                            else if (cantidad > caja.StockIndividual)
                            {
                                var cantidadACajas = (int)Math.Floor(cantidad / caja.Capacidad);

                                var sobra = cantidad % caja.Capacidad;
                                utilidad = (cantidadACajas * caja.Capacidad) * caja.PrecioUnitarioSinIVA + sobra * caja.PrecioUnitarioSinIVA;
                                devolucion.IdCaja = caja.Id;
                                devolucion.IdCarrito = carrito.Id;
                                devolucion.Perdida = carrito.PrecioPublico * cantidad;
                                devolucion.PerdidaSivIva = devolucion.Perdida / 1.16;
                                devolucion.Razon = razon;
                                devolucion.TipoCancelacion = tipoCancelacion;
                                devolucion.Folio = carrito.Folio;
                                devolucion.FechaDevolucion = DateTime.Now.ToString();
                                devolucion.ClaveArticulo = carrito.ClaveArticulo;
                                devolucion.DescripcionArticulo = carrito.DescripcionArticulo;
                                devolucion.Cantidad = sobra;
                                var contenido = new StringContent(JsonSerializer.Serialize(devolucion), Encoding.UTF8);
                                contenido.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                                httpResponseMessage = await httpClient.PostAsync(urlDevoluciones + "AgregarDevolucion/", contenido);
                                var idDevolucion = await httpResponseMessage.Content.ReadAsStringAsync();
                                salidaPorReposicion.CajasSacadas = cantidadACajas;
                                salidaPorReposicion.UnidadesSacadas = sobra;
                                salidaPorReposicion.TotalPerdida = caja.PrecioUnitarioArt * cantidad;
                                salidaPorReposicion.ClaveArticulo = caja.ClaveArticulo;
                                salidaPorReposicion.IdDepartamento = Articulos.Where(b => b.Clave == caja.ClaveArticulo).First().Id;
                                salidaPorReposicion.NombreArticulo = carrito.DescripcionArticulo;
                                salidaPorReposicion.Razon = "Venta";
                                salidaPorReposicion.IdDevolucion = JsonSerializer.Deserialize<Devolucion>(idDevolucion.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Id.ToString();
                                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                                salidaPorReposicion.NombreEmpleado = localSettings.Values["Nombre"] + " " + localSettings.Values["ApPat"] + " " + localSettings.Values["ApMat"];
                                salidaPorReposicion.FechaSalida = DateValue;
                                salidaPorReposicion.IdCaja = caja.Id;

                                salidaPorReposicion.TotalPerdidaSinIva = salidaPorReposicion.TotalPerdida / 1.16;
                                SalidasPorCancelacion.Add(salidaPorReposicion);
                                caja.CajasDisponibles -= cantidadACajas;
                                if (sobra > caja.StockIndividual)
                                {
                                    caja.CajasDisponibles--;
                                    caja.StockIndividual = caja.Capacidad - sobra;

                                }

                            }
                            break;
                        }
                        else if (cantidad > caja.StockTotal && caja.StockTotal != 0)
                        {
                            cantidad = cantidad - caja.StockTotal;
                            var cantidadACajas = (int)Math.Floor(cantidad / caja.Capacidad);
                            var sobra = Math.Floor(cantidad % caja.Capacidad);
                            if (Math.Floor(cantidad % caja.Capacidad) == 0)
                            {

                            }
                            devolucion.IdCaja = caja.Id;
                            devolucion.IdCarrito = carrito.Id;
                            devolucion.Perdida = carrito.PrecioPublico * cantidad;
                            devolucion.PerdidaSivIva = devolucion.Perdida / 1.16;
                            devolucion.Razon = razon;
                            devolucion.TipoCancelacion = tipoCancelacion;
                            devolucion.Folio = carrito.Folio;
                            devolucion.FechaDevolucion = DateTime.Now.ToString();
                            devolucion.ClaveArticulo = carrito.ClaveArticulo;
                            devolucion.DescripcionArticulo = carrito.DescripcionArticulo;
                            devolucion.Cantidad = caja.StockTotal;
                            var contenido = new StringContent(JsonSerializer.Serialize(devolucion), Encoding.UTF8);
                            contenido.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                            httpResponseMessage = await httpClient.PostAsync(urlDevoluciones + "AgregarDevolucion/", contenido);
                            var idDevolucion = await httpResponseMessage.Content.ReadAsStringAsync();
                            salidaPorReposicion.CajasSacadas = cantidadACajas;
                            salidaPorReposicion.UnidadesSacadas = caja.StockTotal;
                            salidaPorReposicion.TotalPerdida = caja.PrecioUnitarioArt * cantidad;
                            salidaPorReposicion.ClaveArticulo = caja.ClaveArticulo;
                            salidaPorReposicion.IdDepartamento = Articulos.Where(b => b.Clave == caja.ClaveArticulo).FirstOrDefault().Id;
                            salidaPorReposicion.NombreArticulo = carrito.DescripcionArticulo;
                            salidaPorReposicion.Razon = "Venta";
                            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                            salidaPorReposicion.NombreEmpleado = localSettings.Values["Nombre"] + " " + localSettings.Values["ApPat"] + " " + localSettings.Values["ApMat"];
                            salidaPorReposicion.FechaSalida = DateValue;
                            salidaPorReposicion.IdCaja = caja.Id;
                            salidaPorReposicion.IdDevolucion = JsonSerializer.Deserialize<Devolucion>(idDevolucion.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Id.ToString();
                            salidaPorReposicion.TotalPerdidaSinIva = salidaPorReposicion.TotalPerdida / 1.16;
                            salidaPorReposicion.NombreDepartamento = ""; //Departamentos.Where(b => b.Id == salidaPorReposicion.IdDepartamento).FirstOrDefault().Nombre;
                            SalidasPorCancelacion.Add(salidaPorReposicion);
                            caja.StockTotal = 0;
                            caja.CajasDisponibles = 0;
                            caja.StockIndividual = 0;
                        }

                    }

                }
                CajasModificadasArticulos.Add(listaCajasPorArticulo);
                var json = JsonSerializer.Serialize(CajasModificadasArticulos[0]);
                var content = new StringContent(json.ToString(), Encoding.UTF8);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                httpResponseMessage = await httpClient.PostAsync(urlCajas + "ActualizarListaCajas/", content);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var me = new MessageDialog("Se realizo la reposicion del articulo: " + carrito.DescripcionArticulo, "Mensaje del sistema");
                    await me.ShowAsync();
                }
                else
                {
                    var me = new MessageDialog("Hubo un prolema al realizar la reposicion " + await httpResponseMessage.Content.ReadAsStringAsync(), "Mensaje del sistema");
                    await me.ShowAsync();
                }
            }
            else
            {
                var me = new MessageDialog("No hay inventario para el la cantidad solicitada del articulo: " + carrito.DescripcionArticulo + "\n" + "Stock disponible: " + stockTotal, "Mensaje del sistema");
                await me.ShowAsync();
                return;
            }




        }
        public async Task CancelarTotalmenteAsync(Carrito carrito, double cantidad, string razon, string tipoCancelacion)
        {

            if (await Cancelar(carrito))
            {
                var md = new MessageDialog("Se cancelo la compra", "Mensaje del sistema");
                var devolucion = new Devolucion();
                devolucion.Razon = razon;
                devolucion.TipoCancelacion = tipoCancelacion;
                devolucion.IdCarrito = carrito.Id;
                var perdida = carrito.PrecioPublico - (carrito.PrecioPublico * (carrito.PorcentajeDescuento / 100));
                devolucion.Perdida = perdida;
                devolucion.PerdidaSivIva = devolucion.Perdida / 1.16;
                devolucion.Cantidad = cantidad;
                devolucion.ClaveArticulo = carrito.ClaveArticulo;
                devolucion.DescripcionArticulo = carrito.DescripcionArticulo;
                devolucion.Folio = carrito.Folio;
                devolucion.FechaDevolucion = DateTime.Now.ToShortDateString();
                var contenido = new StringContent(JsonSerializer.Serialize(devolucion), Encoding.UTF8);
                contenido.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                httpResponseMessage = httpClient.PostAsync(urlDevoluciones + "AgregarDevolucion/", contenido).Result;
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    Total = 0;
                    TotalSinIva = 0;
                    foreach (var vs in VentasSeparadas)
                    {

                        if (vs.Key.Id == carrito.Id)
                        {
                            foreach (var c in vs.Value)
                            {
                                c.RazonCancelado = carrito.RazonCancelado;
                                c.Cantidad = carrito.Cantidad;
                                c.Cancelado = carrito.Cancelado;
                                c.SubTotal = carrito.SubTotal;
                                Total += c.SubTotal;
                                TotalSinIva += c.SubTotal / 1.16;

                            }
                        }
                    }

                    await md.ShowAsync();
                }


            }
        }
        public async Task<List<Devolucion>> GetDevolucionesAsync(string folio)
        {
            httpResponseMessage = await httpClient.GetAsync(urlDevoluciones + "GetDevoluciones/");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var lista = JsonSerializer.Deserialize<List<Devolucion>>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return lista.Where(b => b.Folio == Convert.ToInt32(folio)).ToList();
            }
            else { return null; }
        }
        public async Task<bool> Cancelar(Carrito carrito)
        {
            var content = new StringContent(JsonSerializer.Serialize(carrito), Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpResponseMessage = await httpClient.PostAsync(urlVentas + "CancelarCarrito/", content);


            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
                return false;
        }
        public async Task NuevaVentaAsync()
        {
            CajasModificadasArticulos = new List<List<CajaDispArticulo>>();
            foreach (var itemCompra in Compra)
            {
                var listaCajasDisp = await DisponibilidadArticulo(itemCompra.ClaveArticulo);
                double stockTotal = 0;
                double utilidad = 0;
                var auxCantidad = itemCompra.Cantidad;
                foreach (var caja in listaCajasDisp)
                {
                    stockTotal += caja.StockTotal;
                }
                foreach (var caja in listaCajasDisp)
                {
                    if (stockTotal >= itemCompra.Cantidad)
                    {
                        var carrito = new Carrito();
                        var salidaPorVenta = new SalidaAlmacen();
                        if (auxCantidad <= caja.StockTotal)
                        {
                            caja.StockTotal -= auxCantidad;
                            carrito.Cantidad = auxCantidad;
                            carrito.ClaveArticulo = caja.ClaveArticulo;
                            carrito.DescripcionArticulo = itemCompra.DescripcionArticulo;
                            carrito.MetodoDePago = "";
                            carrito.Folio = 0;
                            carrito.PrecioAlCosto = caja.PrecioUnitarioSinIVA;
                            carrito.PrecioPublico = itemCompra.PrecioPublico;
                            carrito.Utilidad = ((itemCompra.PrecioPublico / 1.16) - caja.PrecioUnitarioSinIVA) * auxCantidad;

                            CompraPorUtilidad.Add(carrito);
                            if (auxCantidad <= caja.StockIndividual)
                            {
                                caja.StockIndividual -= auxCantidad;
                                salidaPorVenta.CajasSacadas = 0;
                                salidaPorVenta.UnidadesSacadas = auxCantidad;
                                salidaPorVenta.TotalPerdida = caja.PrecioUnitarioArt * auxCantidad;
                                salidaPorVenta.ClaveArticulo = caja.ClaveArticulo;
                                salidaPorVenta.IdDepartamento = Articulos.Where(b => b.Clave == caja.ClaveArticulo).First().Id;
                                salidaPorVenta.NombreArticulo = itemCompra.DescripcionArticulo;
                                salidaPorVenta.Razon = "Venta";
                                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                                salidaPorVenta.NombreEmpleado = localSettings.Values["Nombre"] + " " + localSettings.Values["ApPat"] + " " + localSettings.Values["ApMat"];
                                salidaPorVenta.FechaSalida = DateValue;
                                salidaPorVenta.IdCaja = caja.Id;
                                salidaPorVenta.IdDevolucion = "";
                                salidaPorVenta.TotalPerdidaSinIva = salidaPorVenta.TotalPerdida / 1.16;
                                SalidasPorVenta.Add(salidaPorVenta);
                            }
                            else if (itemCompra.Cantidad > caja.StockIndividual)
                            {
                                var cantidadACajas = (int)Math.Floor(auxCantidad / caja.Capacidad);

                                var sobra = auxCantidad % caja.Capacidad;
                                utilidad = (cantidadACajas * caja.Capacidad) * caja.PrecioUnitarioSinIVA + sobra * caja.PrecioUnitarioSinIVA;
                                salidaPorVenta.CajasSacadas = cantidadACajas;
                                salidaPorVenta.UnidadesSacadas = sobra;
                                salidaPorVenta.TotalPerdida = caja.PrecioUnitarioArt * auxCantidad;
                                salidaPorVenta.ClaveArticulo = caja.ClaveArticulo;
                                salidaPorVenta.IdDepartamento = Articulos.Where(b => b.Clave == caja.ClaveArticulo).First().Id;
                                salidaPorVenta.NombreArticulo = itemCompra.DescripcionArticulo;
                                salidaPorVenta.Razon = "Venta";
                                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                                salidaPorVenta.NombreEmpleado = localSettings.Values["Nombre"] + " " + localSettings.Values["ApPat"] + " " + localSettings.Values["ApMat"];
                                salidaPorVenta.FechaSalida = DateValue;
                                salidaPorVenta.IdCaja = caja.Id;
                                salidaPorVenta.IdDevolucion = "";
                                salidaPorVenta.TotalPerdidaSinIva = salidaPorVenta.TotalPerdida / 1.16;
                                SalidasPorVenta.Add(salidaPorVenta);
                                caja.CajasDisponibles -= cantidadACajas;
                                if (sobra > caja.StockIndividual)
                                {
                                    caja.CajasDisponibles--;
                                    caja.StockIndividual = caja.Capacidad - sobra;

                                }

                            }
                            break;
                        }
                        else if (auxCantidad > caja.StockTotal && caja.StockTotal != 0)
                        {
                            auxCantidad = auxCantidad - caja.StockTotal;
                            var cantidadACajas = (int)Math.Floor(auxCantidad / caja.Capacidad);
                            var sobra = Math.Floor(auxCantidad % caja.Capacidad);
                            if (Math.Floor(auxCantidad % caja.Capacidad) == 0)
                            {

                            }
                            salidaPorVenta.CajasSacadas = cantidadACajas;
                            salidaPorVenta.UnidadesSacadas = caja.StockIndividual;
                            salidaPorVenta.TotalPerdida = caja.PrecioUnitarioArt * auxCantidad;
                            salidaPorVenta.ClaveArticulo = caja.ClaveArticulo;
                            salidaPorVenta.IdDepartamento = Articulos.Where(b => b.Clave == caja.ClaveArticulo).First().Id;
                            salidaPorVenta.NombreArticulo = itemCompra.DescripcionArticulo;
                            salidaPorVenta.Razon = "Venta";
                            salidaPorVenta.IdCaja = caja.Id;
                            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                            salidaPorVenta.NombreEmpleado = localSettings.Values["Nombre"] + " " + localSettings.Values["ApPat"] + " " + localSettings.Values["ApMat"];
                            salidaPorVenta.FechaSalida = DateValue;
                            salidaPorVenta.TotalPerdidaSinIva = salidaPorVenta.TotalPerdida / 1.16;
                            salidaPorVenta.NombreDepartamento = Departamentos.Where(b => b.Id == salidaPorVenta.IdDepartamento).First().Nombre;
                            salidaPorVenta.IdDevolucion = "";
                            SalidasPorVenta.Add(salidaPorVenta);
                            carrito.Utilidad = ((itemCompra.PrecioPublico / 1.16) - caja.PrecioUnitarioSinIVA) * caja.StockTotal;
                            carrito.SubTotal = caja.StockTotal * itemCompra.PrecioPublico;
                            carrito.Cantidad = caja.StockTotal;
                            caja.StockTotal = 0;
                            caja.CajasDisponibles = 0;
                            caja.StockIndividual = 0;
                            carrito.ClaveArticulo = caja.ClaveArticulo;
                            carrito.DescripcionArticulo = itemCompra.DescripcionArticulo;
                            carrito.MetodoDePago = "";
                            carrito.Folio = 0;
                            carrito.PrecioAlCosto = caja.PrecioUnitarioSinIVA;
                            carrito.PrecioPublico = itemCompra.PrecioPublico;
                            carrito.RazonCancelado = "";
                            carrito.Cancelado = "No";
                            CompraPorUtilidad.Add(carrito);

                        }

                    }
                    else
                    {
                        var me = new MessageDialog("No hay inventario para el la cantidad solicitada del articulo: " + itemCompra.DescripcionArticulo + "\n" + "Stock disponible: " + stockTotal, "Mensaje del sistema");
                        await me.ShowAsync();
                        return;
                    }
                }
                CajasModificadasArticulos.Add(listaCajasDisp);
            }

            if (Compra.Count > 0)
            {
                foreach (var c in Compra)
                {
                    if (Tarjeta)
                    {
                        c.MetodoDePago = "Tarjeta";
                    }
                    else
                    {
                        c.MetodoDePago = "Efectivo";
                    }
                }
                httpClient = new HttpClient();
                var json = JsonSerializer.Serialize(Compra);
                var content = new StringContent(json, Encoding.UTF8);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                httpResponseMessage = await httpClient.PostAsync(urlVentas + "AgregarVenta", content);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var folio = await httpResponseMessage.Content.ReadAsStringAsync();
                    foreach (var spv in SalidasPorVenta)
                    {
                        spv.FolioVenta = folio.ToString();
                    }
                    if (await ReducirStockPorVentaAsync())
                    {
                        var arr = JsonSerializer.Serialize(SalidasPorVenta);
                        content = new StringContent(arr.ToString(), Encoding.UTF8);
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        httpResponseMessage = await httpClient.PostAsync(urlSalidas + "AgregarSalidasVenta", content);
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            var mens = new MessageDialog("La venta se realizo con el folio: " + folio.ToString(), "Mensaje del Sistema");
                            await mens.ShowAsync();
                            Compra = new ObservableCollection<Carrito>();
                            SalidasPorVenta = new List<SalidaAlmacen>();
                        }


                    }
                }
                //StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                //string dir = localFolder.Path + "\\Ticket.pdf";
                //try
                //{
                //    CrearTicket ticket = new CrearTicket();

                //    ticket.TextoIzquierda(" ");
                //    ticket.TextoCentro("TICKET CIERRE DE CAJA");
                //    ticket.TextoIzquierda(" ");
                //    ticket.TextoExtremos("FECHA : " + DateValue, "HORA : " + DateValue);
                //    ticket.TextoIzquierda(" ");
                //    ticket.EncabezadoVenta();
                //    ticket.lineasGuio();
                //    ticket.lineasIgual();
                //    ticket.AgregarTotales("          TOTAL COMPRADO : $ ",100);
                //    ticket.AgregarTotales("          TOTAL VENDIDO  : $ ",100);
                //    ticket.TextoIzquierda(" ");
                //    ticket.AgregarTotales("          GANANCIA       : $ ",100);
                //    ticket.TextoIzquierda(" ");
                //    ticket.TextoIzquierda(" ");
                //    ticket.TextoIzquierda(" ");
                //    ticket.TextoIzquierda(" ");
                //    ticket.TextoIzquierda(" ");
                //    ticket.TextoIzquierda(" ");
                //    ticket.CortaTicket();
                //    ticket.ImprimirTicket("EPSON TM-T20II Receipt");
                //}
                //catch (Exception eeee) { }
            }
            else
            {
                var mens = new MessageDialog("No hay articulos en el carrito", "Mensaje del Sistema");
                await mens.ShowAsync();
            }

        }

     
        public List<List<CajaDispArticulo>> CajasModificadasArticulos { get; set; }

        bool INotifyDataErrorInfo.HasErrors => throw new NotImplementedException();

        public async void DisponibilidadStockAsync(string clave)
        {
            httpResponseMessage = httpClient.GetAsync(urlCajas + "DisponibilidadArticulo/" + clave).Result;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Lista = JsonSerializer.Deserialize<List<CajaDispArticulo>>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).OrderBy(b => b.PrecioUnitarioSinIVA).ToList();
            }
        }
        public async Task<bool> ReducirStockPorVentaAsync()
        {
            var json = JsonSerializer.Serialize(CajasModificadasArticulos);
            var content = new StringContent(json.ToString(), Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpResponseMessage = await httpClient.PostAsync(urlCajas + "ActualizarTodasCajas/", content);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<CajaDispArticulo>> DisponibilidadArticulo(string clave)
        {
            httpResponseMessage = await httpClient.GetAsync(urlCajas + "DisponibilidadArticulo/" + clave);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<CajaDispArticulo>>(await httpResponseMessage.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).OrderBy(b => b.PrecioUnitarioSinIVA).ToList();
            }
            else
            {
                return new List<CajaDispArticulo>();
            }
        }
        private void ExportarVentasExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("VENTAS");
                worksheet.Cell("B2").Value = "  VENTAS REALIZADAS CON FECHA DESDE :" + Desde.ToString() + " HASTA: " + Hasta.ToString();

                worksheet.Cell("B4").Value = "FOLIO";
                worksheet.Cell("C4").Value = "CLAVE DE ARTICULO";
                worksheet.Cell("D4").Value = "CONCEPTO";
                worksheet.Cell("E4").Value = "CANTIDAD";
                worksheet.Cell("F4").Value = "SUBTOTAL SIN IVA";
                worksheet.Cell("G4").Value = "SUBTOTAL CON IVA 16%";
                var listaFiltrada = Ventas.Where(b => DateTimeOffset.Parse(b.FechaVenta) >= Desde && DateTimeOffset.Parse(b.FechaVenta) <= Hasta && b.Cantidad != 0).ToList();
                var contador = 5;
                double totalCaja = 0;
                double totalTarjeta = 0;
                foreach (var venta in listaFiltrada)
                {
                    worksheet.Cell("B" + contador.ToString()).Value = venta.Folio;
                    worksheet.Cell("C" + contador.ToString()).Value = venta.ClaveArticulo;
                    worksheet.Cell("D" + contador.ToString()).Value = venta.DescripcionArticulo;
                    worksheet.Cell("E" + contador.ToString()).Value = venta.Cantidad;
                    worksheet.Cell("F" + contador.ToString()).Value = venta.SubTotalSinIva;
                    worksheet.Cell("G" + contador.ToString()).Value = venta.SubTotal;
                    contador++;
                    if (venta.MetodoDePago == "Efectivo")
                    {
                        totalCaja += venta.SubTotal;
                    }
                    else
                    {
                        totalTarjeta += venta.SubTotal;
                    }
                }
                var cellTotal = worksheet.Cell("F" + (contador).ToString());
                var cellTotalSinIva = worksheet.Cell("G" + (contador).ToString());
                cellTotal.FormulaA1 = "SUM(F5:F" + (contador - 1).ToString() + ")";
                cellTotalSinIva.FormulaA1 = "SUM(G5:G" + (contador - 1).ToString() + ")";


                var range = worksheet.Range(worksheet.Cell("B4"), worksheet.LastCellUsed());
                worksheet.Cell("B" + (contador + 2).ToString()).Value = "Total en caja: $" + totalCaja.ToString();
                worksheet.Cell("C" + (contador + 2).ToString()).Value = "Total en tarjeta: $" + totalTarjeta.ToString();
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var table = range.CreateTable();
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 30;
                worksheet.Column(7).Width = 30;
                worksheet.Style.Font.SetFontName("Arial");
                worksheet.Style.Font.SetFontSize(12);
                worksheet.Style.Font.FontFamilyNumbering = XLFontFamilyNumberingValues.Roman;
                string dir = localFolder.Path + "\\Ventas.xlsx";
                workbook.SaveAs(dir);
                var md = new MessageDialog("Excel creado correctamente!", "Mensaje del Sistema");
                md.ShowAsync();

            }
        }
        private void ExportarAExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DEVOLUCIONES");
                var cont = 4;
                double totalDeDevolucion = 0;
                double totalDevolucionSinIva = 0;
                var contFac = 0;
                worksheet.Cell("B1").Value = "DEVOLUCIONES REALIZADAS DESDE: " + Desde.ToString() + " hasta: " + Hasta.ToString();
                worksheet.Cell("A3").Value = "Numero de devolucion";
                worksheet.Cell("B3").Value = "Folio de venta";
                worksheet.Cell("C3").Value = "Tipo de devolucion";
                worksheet.Cell("D3").Value = "Razon de la devolucion";
                worksheet.Cell("E3").Value = "Total devuelto sin IVA";
                worksheet.Cell("F3").Value = "Total devuelto con IVA 16%";
                foreach (var devolucion in ListaDevoluciones)
                {
                    worksheet.Cell("A" + cont.ToString()).Value = devolucion.Id;
                    worksheet.Cell("B" + cont.ToString()).Value = devolucion.Folio;
                    worksheet.Cell("C" + cont.ToString()).Value = devolucion.TipoCancelacion;
                    worksheet.Cell("D" + cont.ToString()).Value = devolucion.Razon;
                    worksheet.Cell("E" + cont.ToString()).Value = devolucion.PerdidaSivIva;
                    worksheet.Cell("F" + cont.ToString()).Value = devolucion.Perdida;
                    totalDeDevolucion += devolucion.Perdida;
                    totalDevolucionSinIva += devolucion.PerdidaSivIva;
                    cont++;

                }

                worksheet.Cell("E" + cont.ToString()).Value = "Total de perdida sin IVA: " + "$" + totalDevolucionSinIva;
                worksheet.Cell("F" + cont.ToString()).Value = "Total de perdida: " + "$" + totalDeDevolucion;
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 30;
                worksheet.Column(7).Width = 30;
                var range = worksheet.Range(worksheet.Cell("A3"), worksheet.LastCellUsed());
                worksheet.Style.Font.SetFontName("Arial");
                worksheet.Style.Font.SetFontSize(12);
                worksheet.Style.Font.FontFamilyNumbering = XLFontFamilyNumberingValues.Roman;
                var table = range.CreateTable("DEVOLUCIONES REALIZADAS DESDE: " + Desde.ToString() + " hasta: " + Hasta.ToString());

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                string dir = localFolder.Path + "\\Devoluciones.xlsx";
                try
                {
                    workbook.SaveAs(dir);
                    var md = new MessageDialog("Excel generado correctamente", "Mensaje del Sistema");
                    md.ShowAsync();
                }
                catch (Exception e)
                {
                    var md = new MessageDialog("Hubo un problema al generar el archivo " + e.Message, "Mensaje del Sistema");
                    md.ShowAsync();
                }

            }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
