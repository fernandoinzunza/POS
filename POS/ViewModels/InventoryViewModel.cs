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
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;
using Windows.Storage;

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
        private const string urlSalidas = "http://localhost:9095/api/SalidasAlmacen/";
        private const string urlCajas = "http://localhost:9095/api/CajaDispArticulos/";
        private const string urlCompras = "http://localhost:9095/api/CompraAProveedors/";
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
        private ObservableCollection<EntradaGrid> entradaTabla;
        private Dictionary<EntradaAlmacen, RelayCommand> listaConComando;
        private bool porUnidad;
        private double precioXCaja;
        private double stockIndividualAEntrar;
        private Dictionary<EntradaAlmacen, List<EntradaAlmacen>> comprasAlmacen;
        private Dictionary<string, List<EntradaAlmacen>> entradasTemporal;
        private List<CajaDispArticulo> cajasDisp;
        private List<CajaDispArticulo> cajasPorArticulo;
        private Dictionary<KeyValuePair<Articulo, string>, List<CajaDispArticulo>> disponibilidad;
        private DateTimeOffset desde;
        private DateTimeOffset hasta;
        private List<string> listaFacturas;
        private List<EntradaAlmacen> articulosPorFactura;
        private List<EntradaAlmacen> cajasCompradas;
        private ObservableCollection<EntradaAlmacen> reposiciones;
        private DateTimeOffset desdeReposicion;
        private DateTimeOffset hastaReposicion;

        public List<string> ListaFacturas
        {
            get { return listaFacturas; }
            set { SetProperty(ref listaFacturas, value); }
        }
        public ObservableCollection<EntradaAlmacen> Reposiciones
        {
            get { return reposiciones; }
            set { SetProperty(ref reposiciones, value); }
        }
        public DateTimeOffset DesdeReposicion
        {
            get { return desdeReposicion; }
            set
            {
                SetProperty(ref desdeReposicion, value);

                Reposiciones = new ObservableCollection<EntradaAlmacen>(Entradas.Where(b => b.PrecioXCaja == 0 && DateTimeOffset.Parse(b.Fecha) >= DesdeReposicion && DateTimeOffset.Parse(b.Fecha) <= HastaReposicion));

            }
        }
        public DateTimeOffset HastaReposicion
        {
            get { return hastaReposicion; }
            set
            {
                SetProperty(ref hastaReposicion, value);

                Reposiciones = new ObservableCollection<EntradaAlmacen>(Entradas.Where(b => b.PrecioXCaja == 0 && DateTimeOffset.Parse(b.Fecha) >= DesdeReposicion && DateTimeOffset.Parse(b.Fecha) <= HastaReposicion));

            }
        }
        public List<EntradaAlmacen> ArticulosPorFactura
        {
            get { return articulosPorFactura; }
            set { SetProperty(ref articulosPorFactura, value); }
        }
        public List<EntradaAlmacen> CajasCompradas
        {
            get { return cajasCompradas; }
            set { SetProperty(ref cajasCompradas, value); }
        }



        public RelayCommand EntradaGrid { get => new RelayCommand(o => { NuevaEntrada(); }); }

        public ObservableCollection<EntradaGrid> EntradaTabla
        {
            get { return entradaTabla; }
            set { SetProperty(ref entradaTabla, value); }
        }
        public Dictionary<EntradaAlmacen, RelayCommand> ListaConComando
        {
            get { return ListaConComando; }
            set { SetProperty(ref listaConComando, value); }
        }
        public List<CajaDispArticulo> CajasDisp
        {
            get; set;
        }
        public List<CajaDispArticulo> CajasPorArticulo
        {
            get { return cajasPorArticulo; }
            set { SetProperty(ref cajasPorArticulo, value); }
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

        private object facturaSeleccionada;
        private object articuloSeleccionado;

        public object FacturaSeleccionada
        {
            get { return facturaSeleccionada; }
            set
            {
                SetProperty(ref facturaSeleccionada, value);
                if (FacturaSeleccionada != null)
                {
                    var fs = FacturaSeleccionada.ToString();
                    ArticulosPorFactura = new List<EntradaAlmacen>();
                    ArticulosPorFactura = Entradas.Where(b => b.NoFactura == fs).GroupBy(b => b.ClaveArticulo).Select(g => g.FirstOrDefault()).ToList();
                }
            }
        }
        public object ArticuloSeleccionado
        {
            get { return articuloSeleccionado; }
            set
            {
                SetProperty(ref articuloSeleccionado, value);
                if (ArticuloSeleccionado != null)
                {
                    var fs = FacturaSeleccionada.ToString();
                    var art = ArticuloSeleccionado as EntradaAlmacen;
                    CajasCompradas = new List<EntradaAlmacen>();
                    CajasCompradas = Entradas.Where(b => b.NoFactura == fs && b.ClaveArticulo == art.ClaveArticulo && b.PrecioXCaja != 0).ToList();
                }
            }
        }
        private object cajaSeleccionada;
        public object CajaSeleccionada
        {
            get { return cajaSeleccionada; }
            set { SetProperty(ref cajaSeleccionada, value); }
        }
        public List<EntradaAlmacen> Entradas
        {
            get; set;
        }
        public Dictionary<EntradaAlmacen, List<EntradaAlmacen>> ComprasAlmacen
        {
            get { return comprasAlmacen; }
            set { SetProperty(ref comprasAlmacen, value); }
        }
        public Dictionary<KeyValuePair<Articulo, string>, List<CajaDispArticulo>> Disponibilidad
        {
            get { return disponibilidad; }
            set { SetProperty(ref disponibilidad, value); }
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
        public DateTimeOffset Desde
        {
            get { return desde; }
            set
            {
                SetProperty(ref desde, value);
                httpClient = new HttpClient();
                HttpResponseMessage mensaje = httpClient.GetAsync(urlEntradas + "GetEntradasAlmacen/").Result;
                ComprasAlmacen = new Dictionary<EntradaAlmacen, List<EntradaAlmacen>>();
                if (mensaje.IsSuccessStatusCode)
                {
                    var lista = mensaje.Content.ReadAsStringAsync().Result;
                    var aux = JsonSerializer.Deserialize<List<EntradaAlmacen>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var aux2 = aux.GroupBy(b => b.NoFactura).Select(g => g.FirstOrDefault()).Where(b => DateTimeOffset.Parse(b.Fecha) >= Desde && DateTimeOffset.Parse(b.Fecha) <= Hasta && b.PrecioXCaja != 0).ToList();
                    foreach (var a in aux2)
                    {
                        var listaPorFactura = aux.Where(b => b.NoFactura == a.NoFactura && b.PrecioXCaja != 0).ToList();
                        ComprasAlmacen.Add(a, listaPorFactura);
                    }
                }
            }
        }
        private DateTimeOffset desdeSalida;
        public DateTimeOffset DesdeSalida
        {
            get { return desdeSalida; }
            set
            {
                SetProperty(ref desdeSalida, value);
                httpClient = new HttpClient();
                SalidasAlmacen = new ObservableCollection<SalidaAlmacen>(); ;
                HttpResponseMessage mensaje = httpClient.GetAsync(urlSalidas + "GetSalidas/").Result;
                if (mensaje.IsSuccessStatusCode)
                {
                    var aux = JsonSerializer.Deserialize<ObservableCollection<SalidaAlmacen>>(mensaje.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
        }
        private DateTimeOffset hastaSalida;
        public DateTimeOffset HastaSalida
        {
            get { return hastaSalida; }
            set
            {
                SetProperty(ref hastaSalida, value);
                httpClient = new HttpClient();
                SalidasAlmacen = new ObservableCollection<SalidaAlmacen>();
                HttpResponseMessage mensaje = httpClient.GetAsync(urlSalidas + "GetSalidas/").Result;
                if (mensaje.IsSuccessStatusCode)
                {
                    var aux = JsonSerializer.Deserialize<ObservableCollection<SalidaAlmacen>>(mensaje.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    SalidasAlmacen = new ObservableCollection<SalidaAlmacen>(aux.Where(b => DateTimeOffset.Parse(b.FechaSalida) >= DesdeSalida && DateTimeOffset.Parse(b.FechaSalida) <= HastaSalida).ToList());
                }
            }
        }
        public DateTimeOffset Hasta
        {
            get { return hasta; }
            set
            {
                SetProperty(ref hasta, value);
                httpClient = new HttpClient();
                HttpResponseMessage mensaje = httpClient.GetAsync(urlEntradas + "GetEntradasAlmacen/").Result;
                ComprasAlmacen = new Dictionary<EntradaAlmacen, List<EntradaAlmacen>>();
                if (mensaje.IsSuccessStatusCode)
                {
                    var lista = mensaje.Content.ReadAsStringAsync().Result;
                    var aux = JsonSerializer.Deserialize<List<EntradaAlmacen>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    var aux2 = aux.GroupBy(b => b.NoFactura).Select(g => g.FirstOrDefault()).Where(b => DateTimeOffset.Parse(b.Fecha) >= Desde && DateTimeOffset.Parse(b.Fecha) <= Hasta && b.PrecioXCaja != 0).ToList();
                    foreach (var a in aux2)
                    {
                        var listaPorFactura = aux.Where(b => b.NoFactura == a.NoFactura && b.PrecioXCaja != 0).ToList();
                        ComprasAlmacen.Add(a, listaPorFactura);
                    }
                }
            }
        }
        public object ComboDepartamento
        {
            get { return comboDepartamento; }
            set
            {
                SetProperty(ref comboDepartamento, value);
                if (ComboDepartamento != null)
                {
                    var combo = ComboDepartamento as Departamento;
                    var id = combo.Id;
                    ArticulosPorDepartamento = Articulos.Where(b => b.IdDepartamento == id).ToList();
                }


            }
        }

        public object ComboArticulo
        {
            get { return comboArticulo; }
            set
            {
                SetProperty(ref comboArticulo, value);
                if (ComboArticulo != null)
                {
                    CajasPorArticulo = CajasDisp.Where(b => b.ClaveArticulo == (ComboArticulo as Articulo).Clave).ToList();
                }
            }
        }
        private object comboCajas;
        public object ComboCajas
        {
            get { return comboCajas; }
            set
            {
                SetProperty(ref comboCajas, value);
                if (ComboCajas != null)
                {
                    StockTotal = (ComboCajas as CajaDispArticulo).StockTotal;
                    StockCaja = (ComboCajas as CajaDispArticulo).CajasDisponibles;
                    var caja = CajasPorArticulo.Where(b => b.ClaveArticulo == (ComboArticulo as Articulo).Clave && b.Capacidad == (ComboCajas as CajaDispArticulo).Capacidad && b.PrecioXCaja == (ComboCajas as CajaDispArticulo).PrecioXCaja).First();
                    StockIndividual = caja.StockIndividual;
                    if ((ComboCajas as CajaDispArticulo).Capacidad == 1)
                    {
                        Individual = false;
                    }
                    else
                    {
                        Individual = true;
                    }
                }

            }
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
            set
            {
                SetProperty(ref precioXCaja, value);
                if (PorCaja)
                {
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0 && PrecioXCaja != 0)
                    {
                        PrecioUnitario = PrecioXCaja / UnidadXCajas;
                        CostoTotal = CantidadCajasCompradas * PrecioXCaja + (PrecioUnitario * StockIndividualAEntrar);
                    }
                }
                if (UnidadXCajas == 1 && !PorCaja)
                {
                    PrecioUnitario = PrecioXCaja;
                    CostoTotal = StockIndividualAEntrar * PrecioXCaja;
                }
            }
        }
        public double StockIndividualAEntrar
        {
            get { return stockIndividualAEntrar; }
            set
            {
                SetProperty(ref stockIndividualAEntrar, value);
                if (PorCaja)
                {
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0)
                        Cantidad = UnidadXCajas * CantidadCajasCompradas;
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0 && PrecioXCaja != 0)
                    {
                        PrecioUnitario = PrecioXCaja / UnidadXCajas;
                        CostoTotal = CantidadCajasCompradas * PrecioXCaja + (PrecioUnitario * StockIndividualAEntrar);
                    }

                }
                if (UnidadXCajas == 1 && !PorCaja)
                {
                    PrecioUnitario = PrecioXCaja;
                    CostoTotal = StockIndividualAEntrar * PrecioXCaja;
                }
            }
        }
        public double UnidadXCajas
        {
            get { return unidadXCajas; }
            set
            {
                SetProperty(ref unidadXCajas, value);
                if (PorCaja)
                {
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0)
                        Cantidad = UnidadXCajas * CantidadCajasCompradas;
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0 && PrecioXCaja != 0)
                    {
                        PrecioUnitario = PrecioXCaja / UnidadXCajas;
                        CostoTotal = CantidadCajasCompradas * PrecioXCaja + (PrecioUnitario * StockIndividualAEntrar);
                    }
                    if (UnidadXCajas == 1 && !PorCaja)
                    {
                        PrecioUnitario = PrecioXCaja;
                        CostoTotal = StockIndividualAEntrar * PrecioXCaja;
                    }
                }

            }
        }
        public double CantidadCajasCompradas
        {
            get { return cantidadCajasCompradas; }
            set
            {
                SetProperty(ref cantidadCajasCompradas, value);
                if (PorCaja)
                {
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0)
                        Cantidad = UnidadXCajas * CantidadCajasCompradas;
                    if (UnidadXCajas != 0 && CantidadCajasCompradas != 0 && PrecioXCaja != 0)
                    {
                        CostoTotal = CantidadCajasCompradas * PrecioXCaja;
                        PrecioUnitario = PrecioXCaja / UnidadXCajas;
                    }
                    if (UnidadXCajas == 1 && !PorCaja)
                    {
                        PrecioUnitario = PrecioXCaja;
                        CostoTotal = StockIndividualAEntrar * PrecioXCaja;
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
                if (!PorCaja)
                {
                    UnidadXCajas = 1;
                }
            }

        }
        public bool PorUnidad
        {
            get { return porUnidad; }
            set
            {
                SetProperty(ref porUnidad, value);
            }
        }

        #region Salida
        private string razon;
        private int stockCaja;
        private double stockIndividual;
        private double stockTotal;
        private int cajasASacar;
        private double stockIndividualASacar;
        private double totalPerdida;

        public string Razon
        {
            get { return razon; }
            set { SetProperty(ref razon, value); }
        }
        public int StockCaja
        {
            get { return stockCaja; }
            set { SetProperty(ref stockCaja, value); }
        }
        private bool individual;
        public bool Individual
        {
            get { return individual; }
            set { SetProperty(ref individual, value); }
        }
        public double StockIndividual
        {
            get { return stockIndividual; }
            set { SetProperty(ref stockIndividual, value); }
        }
        public double StockTotal
        {
            get { return stockTotal; }
            set { SetProperty(ref stockTotal, value); }
        }
        private ObservableCollection<SalidaAlmacen> salidasAlmacen;
        public ObservableCollection<SalidaAlmacen> SalidasAlmacen
        {
            get { return salidasAlmacen; }
            set { SetProperty(ref salidasAlmacen, value); }
        }
        public int CajasASacar
        {
            get { return cajasASacar; }
            set
            {
                SetProperty(ref cajasASacar, value);
                if (CajasASacar != 0)
                {
                    PorCaja = true;
                }
                else
                    PorCaja = false;

                if(ComboArticulo != null)
                {
                    if (!PorCaja)
                    {
                        var caja = CajasPorArticulo.Where(b => b.ClaveArticulo == (ComboCajas as CajaDispArticulo).ClaveArticulo).First();
                        var perdidaIndividual = stockIndividualASacar * caja.PrecioUnitarioArt;
                        TotalPerdida = (caja.PrecioXCaja * CajasASacar) + perdidaIndividual;
                    }
                    else
                    {
                        var articulo = Articulos.Where(b => b.Clave == (ComboArticulo as Articulo).Clave).First();
                        var perdidaIndividual = stockIndividualASacar * articulo.PrecioAlCosto;

                        TotalPerdida = (articulo.PrecioXCaja * CajasASacar) + perdidaIndividual;

                    }
                }
               else
                {
                    var md = new MessageDialog("Selecciona el articulo a sacar!", "Mensaje del Sistema");
                    md.ShowAsync();
                }
            }
        }
        public double StockIndividualASacar
        {
            get { return stockIndividualASacar; }
            set
            {
                SetProperty(ref stockIndividualASacar, value);
                if (ComboArticulo != null)
                {
                    if (!PorCaja)
                    {
                        var caja = CajasPorArticulo.Where(b => b.ClaveArticulo == (ComboArticulo as Articulo).Clave && b.Capacidad == (ComboCajas as CajaDispArticulo).Capacidad && b.PrecioXCaja == (ComboCajas as CajaDispArticulo).PrecioXCaja).First();
                        var perdidaIndividual = StockIndividualASacar * caja.PrecioUnitarioArt;
                        TotalPerdida = (caja.PrecioXCaja * CajasASacar) + perdidaIndividual;
                    }
                    else
                    {
                        var caja = CajasPorArticulo.Where(b => b.ClaveArticulo == (ComboArticulo as Articulo).Clave && b.Capacidad == (ComboCajas as CajaDispArticulo).Capacidad && b.PrecioXCaja == (ComboCajas as CajaDispArticulo).PrecioXCaja).First();
                        var perdidaIndividual = StockIndividualASacar * caja.PrecioUnitarioArt;
                        TotalPerdida = (caja.PrecioXCaja * CajasASacar) + perdidaIndividual;

                    }
                }
                else
                {
                    var md = new MessageDialog("Selecciona el articulo a sacar!", "Mensaje del Sistema");
                    md.ShowAsync();
                }
            }
        }
        public double TotalPerdida
        {
            get { return totalPerdida; }
            set { SetProperty(ref totalPerdida, value); }
        }
        public RelayCommand RegistrarSalida { get; set; }
        private async Task NuevaSalidaAsync()
        {
            var alerta = new MessageDialog("", "Mensaje del sistema");
            if (Razon == null)
            {
                alerta.Content = "Agrega una razon para la salida del inventario!";
                await alerta.ShowAsync();
            }
            else if (ComboDepartamento == null)
            {
                alerta.Content = "Selecciona el departamento del producto!";
                await alerta.ShowAsync();
            }
            else if (ComboArticulo == null)
            {
                alerta.Content = "Selecciona el articulo a sacar!";
                await alerta.ShowAsync();
            }
            else if (ComboCajas == null)
            {
                alerta.Content = "Selecciona la caja de donde va salir el inventario!";
                await alerta.ShowAsync();
            }
            else if (CajasASacar == 0 && StockIndividualASacar == 0)
            {
                alerta.Content = "Para dar salida, indica al menos una unidad del inventario!";
                await alerta.ShowAsync();
            }
            else
            {


                var articulo = Articulos.Where(b => b.Clave == (ComboArticulo as Articulo).Clave).First();
                var caja = CajasPorArticulo.Where(b => b.Id == (ComboCajas as CajaDispArticulo).Id && b.Capacidad == (ComboCajas as CajaDispArticulo).Capacidad && (ComboCajas as CajaDispArticulo).PrecioXCaja == b.PrecioXCaja).First();
                var totalStockSalida = (CajasASacar * caja.Capacidad) + StockIndividualASacar;
                var combo = (ComboCajas as CajaDispArticulo);
                if (caja.StockTotal >= totalStockSalida)
                {
                    var salida = new SalidaAlmacen();
                    var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                    salida.CajasSacadas = CajasASacar;
                    salida.ClaveArticulo = (ComboArticulo as Articulo).Clave;
                    salida.IdDepartamento = (ComboDepartamento as Departamento).Id;
                    salida.NombreArticulo = (ComboArticulo as Articulo).Descripcion;
                    salida.Razon = Razon;
                    salida.TotalPerdida = TotalPerdida;
                    salida.IdCaja = caja.Id;
                    salida.UnidadesSacadas = StockIndividualASacar;
                    salida.FechaSalida = DateValue;
                    salida.NombreEmpleado = localSettings.Values["Nombre"] + " " + localSettings.Values["ApPat"] + " " + localSettings.Values["ApMat"];
                    salida.FolioVenta = "";
                    salida.IdDevolucion = "";
                    var content = new StringContent(JsonSerializer.Serialize(salida), Encoding.UTF8);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage respuesta = await httpClient.PostAsync(urlSalidas + "AgregarSalida", content);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        var unidadesXCaja = caja.Capacidad;
                        if (StockIndividualASacar > unidadesXCaja && unidadesXCaja != 0 && unidadesXCaja != 1)
                        {
                            var res = (StockIndividualASacar - caja.StockIndividual) % unidadesXCaja;
                            var division = (StockIndividualASacar - caja.StockIndividual) / unidadesXCaja;
                            var stockIndACajas = Math.Floor(division);
                            var residuo = (StockIndividualASacar - caja.StockIndividual) % unidadesXCaja;
                            var fueraDeCaja = residuo;
                            CajasASacar += (int)stockIndACajas;
                            StockIndividualASacar = residuo;
                            caja.CajasDisponibles = (int)(caja.CajasDisponibles - CajasASacar);
                            caja.StockTotal -= (float)totalStockSalida;
                            if (residuo > caja.StockIndividual)
                            {
                                caja.CajasDisponibles--;
                                caja.StockIndividual = caja.Capacidad - residuo;
                            }
                        }
                        else if (StockIndividualASacar <= unidadesXCaja && unidadesXCaja != 0 && unidadesXCaja != 1)
                        {
                            if (StockIndividualASacar > caja.StockIndividual)
                            {
                                caja.StockIndividual = (unidadesXCaja) - (StockIndividualASacar - caja.StockIndividual);
                                if (CajasASacar == 0)
                                    caja.CajasDisponibles--;
                                else
                                {
                                    caja.CajasDisponibles -= (CajasASacar + 1);
                                }
                                caja.StockTotal -= totalStockSalida;
                            }
                            else if (StockIndividualASacar == 0 && CajasASacar > 0)
                            {
                                caja.CajasDisponibles -= CajasASacar;
                                caja.StockTotal -= (float)CajasASacar * (float)caja.Capacidad;
                            }
                            else
                            {
                                caja.StockIndividual = caja.StockIndividual - StockIndividualASacar;
                                caja.StockTotal = caja.StockTotal - StockIndividualASacar;
                            }

                        }
                        else if (unidadesXCaja == 0 || unidadesXCaja == 1)
                        {
                            caja.StockTotal -= (float)StockIndividualASacar;
                            caja.StockIndividual -= (float)StockIndividualASacar;
                        }
                        content = new StringContent(JsonSerializer.Serialize(caja), Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        respuesta = await httpClient.PostAsync(urlCajas + "UpdateCaja/", content);
                        if (respuesta.IsSuccessStatusCode)
                        {
                            var messageDialog = new MessageDialog("Se registro la salida correctamente", "Mensaje del Sistema");
                            await messageDialog.ShowAsync();
                            CajasASacar = 0;
                            Razon = "";
                            StockIndividual = 0;
                            StockIndividualASacar = 0;
                            StockTotal = 0;
                            ComboArticulo = null;
                            ComboDepartamento = null;
                            ComboCajas = null;

                        }
                    }
                }
                else
                {
                    var messageDialog = new MessageDialog("Hay menos stock en inventario que el que intentas sacar", "Error");
                    await messageDialog.ShowAsync();
                }

            }
        }
        private int cajasAreponer;
        private double unidadesAReponer;

        public int CajasAReponer { get { return cajasAreponer; } set { SetProperty(ref cajasAreponer, value); } }
        public double UnidadesAReponer { get { return unidadesAReponer; } set { SetProperty(ref unidadesAReponer, value); } }
        #endregion

        public RelayCommand AgregarEntrada { get; set; }
        public RelayCommand BorrarLista { get; set; }
        public RelayCommand NuevaEntradaPorReposicion { get; set; }

        public Dictionary<Articulo, bool> Stocks { get { return stocks; } set { SetProperty(ref stocks, value); } }
        public InventoryViewModel()
        {
            Individual = true;
            MismaFactura = true;
            Nueva = false;
            TotalCompra = 0;
            PorCaja = true;
            PrecioXCaja = 0;
            PorUnidad = false;
            ListaTemporal = new ObservableCollection<EntradaAlmacen>();
            ListaConComando = new Dictionary<EntradaAlmacen, RelayCommand>();
            EntradasTemporal = new Dictionary<string, List<EntradaAlmacen>>();
            EntradaTabla = new ObservableCollection<EntradaGrid>();
            Articulos = new List<Articulo>();
            ArticulosPorDepartamento = new List<Articulo>();
            Disponibilidad = new Dictionary<KeyValuePair<Articulo, string>, List<CajaDispArticulo>>();
            DateTimeOffset dateTime;
            DateTimeOffset.TryParse("01/01/2021", out dateTime);
            Desde = dateTime;
            Hasta = DateTimeOffset.Now.Date;
            CantidadCajasCompradas = 0;
            DesdeSalida = dateTime;
            HastaSalida = DateTimeOffset.Now.Date;
            Stocks = new Dictionary<Articulo, bool>();
            Colores = new Dictionary<Articulo, string>();
            httpClient = new HttpClient();
            Cantidad = 0.0;
            PrecioUnitario = 0.0;
            CostoTotal = 0.0;
            CajasDisp = new List<CajaDispArticulo>();
            CajasAReponer = 0;
            UnidadesAReponer = 0;
            DateValue = DateTime.Now.ToShortDateString();
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
            mensaje = httpClient.GetAsync(urlCajas + "GetCajasDisp").Result;
            if (mensaje.IsSuccessStatusCode)
            {
                var lista = mensaje.Content.ReadAsStringAsync().Result;
                CajasDisp = JsonSerializer.Deserialize<List<CajaDispArticulo>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            mensaje = httpClient.GetAsync(urlSalidas + "GetSalidas/").Result;
            if (mensaje.IsSuccessStatusCode)
            {
                var lista = mensaje.Content.ReadAsStringAsync().Result;
                SalidasAlmacen = JsonSerializer.Deserialize<ObservableCollection<SalidaAlmacen>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            /*
            mensaje = httpClient.GetAsync(urlEntradas + "GetEntradasAlmacen/").Result;
            if (mensaje.IsSuccessStatusCode)
            {
                var lista = mensaje.Content.ReadAsStringAsync().Result;
                var aux = JsonSerializer.Deserialize<List<EntradaAlmacen>>(lista.ToString(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var aux2 = aux.GroupBy(b => b.NoFactura).Select(g => g.FirstOrDefault()).Where(b => DateTimeOffset.Parse(b.Fecha) >= Desde && DateTime.Parse(b.Fecha) >= Hasta).ToList();
                foreach (var a in aux2)
                {
                    var listaPorFactura = aux.Where(b => b.NoFactura == a.NoFactura).ToList();
                    ComprasAlmacen.Add(a, listaPorFactura);
                }
            }
            */
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
            RegistrarSalida = new RelayCommand(async (o) => { await NuevaSalidaAsync(); });
            foreach (var a in Articulos)
            {
                var listaDeCaja = new List<CajaDispArticulo>();
                listaDeCaja = CajasDisp.Where(b => b.ClaveArticulo == a.Clave).ToList();
                double stockTotal = 0;
                foreach (var c in listaDeCaja)
                {
                    stockTotal += c.StockTotal;
                }
                var stockbajo = a.StockBajo;
                var color = "";
                if (stockTotal <= stockbajo)
                {
                    color = "#f54242";
                }
                else
                {
                    double porcentaje = stockbajo * 100 / stockTotal;
                    if (porcentaje <= 25)
                    {
                        color = "#2eb82e";
                    }
                    else if (porcentaje <= 50)
                    {
                        color = "#ffff33";
                    }
                    else if (porcentaje <= 75)
                    {
                        color = "#ff8533";
                    }

                }
                KeyValuePair<Articulo, string> dic = new KeyValuePair<Articulo, string>(a, color);


                Disponibilidad.Add(dic, listaDeCaja);

            }
            CrearExcel = new RelayCommand(o => { ExcelCreated(); });
            CrearExcelSalidas = new RelayCommand(o => { SalidasExcel(); });
            ListaFacturas = new List<string>();
            HttpResponseMessage httpResponse = httpClient.GetAsync(urlEntradas + "GetEntradasAlmacen/").Result;
            if (httpResponse.IsSuccessStatusCode)
            {
                Entradas = JsonSerializer.Deserialize<List<EntradaAlmacen>>(httpResponse.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                foreach (var e in Entradas)
                {
                    if (!ListaFacturas.Contains(e.NoFactura))
                        ListaFacturas.Add(e.NoFactura);
                }
            }
            DateTimeOffset dateTimeDesde;
            DateTimeOffset.TryParse("01/01/2021", out dateTimeDesde);
            DesdeReposicion = dateTimeDesde;
            HastaReposicion = DateTimeOffset.Now;
            NuevaEntradaPorReposicion = new RelayCommand(async (o) =>
           {
               await EntradaPorReposicion();
           });
            ExportarReposiciones = new RelayCommand(o =>
            {
                ReposicionesAExcel();
            });
        }
        public RelayCommand CrearExcel
        {
            get; set;
        }
        public RelayCommand ExportarReposiciones { get; set; }
        public RelayCommand CrearExcelSalidas
        {
            get; set;
        }
        private void ReposicionesAExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ENTRADAS POR REPOSICION");
                var cont = 4;
                double totalDeTotales = 0;
                var contFac = 0;
                worksheet.Cell("B2").Value = "ENTRADAS POR REPOSICION CON FECHA DESDE " + DesdeReposicion.Date.ToShortDateString() + " HASTA " + HastaReposicion.Date.ToShortDateString();
                worksheet.Cell("B4").Value = "Proviene de factura";
                worksheet.Cell("C4").Value = "Empleado que registro";
                worksheet.Cell("D4").Value = "Fecha de entrada:";
                worksheet.Cell("E4").Value = "Proveedor que repone:";
                worksheet.Cell("F4").Value = "Clave del articulo:";
                worksheet.Cell("G4").Value = "Concepto:";
                worksheet.Cell("H4").Value = "Cajas repuestas:";
                worksheet.Cell("I4").Value = "Unidades repuestas:";
                worksheet.Cell("J4").Value = "Cantidad total repuesta:";
                foreach (var c in Reposiciones)
                {
                    var art = Articulos.Where(b => b.Clave == c.ClaveArticulo).FirstOrDefault();
                    worksheet.Cell("B" + (cont + 1).ToString()).Value = c.NoFactura;
                    worksheet.Cell("C" + (cont + 1).ToString()).Value = c.NombreEmpleado;
                    worksheet.Cell("D" + (cont + 1).ToString()).Value = c.Fecha;
                    worksheet.Cell("E" + (cont + 1).ToString()).Value = c.NombreProveedor;
                    worksheet.Cell("F" + (cont + 1).ToString()).Value = c.ClaveArticulo;
                    worksheet.Cell("G" + (cont + 1).ToString()).Value = c.Articulo;
                    worksheet.Cell("H" + (cont + 1).ToString()).Value = c.CajasStock + " (" + c.UnidadesXCaja + " " + art.Unidad + ")";
                    worksheet.Cell("I" + (cont + 1).ToString()).Value = c.StockIndividual + " " + art.Unidad;
                    worksheet.Cell("J" + (cont + 1).ToString()).Value = c.Cantidad + " " + art.Unidad;
                    cont++;

                }
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 30;
                worksheet.Column(7).Width = 30;
                worksheet.Column(8).Width = 30;
                worksheet.Column(9).Width = 30;
                worksheet.Column(10).Width = 30;

                worksheet.Style.Font.SetFontName("Arial");
                worksheet.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Style.Font.SetFontSize(12);
                var range = worksheet.Range(worksheet.Cell("B4"), worksheet.LastCellUsed());
                range.CreateTable();
                worksheet.Style.Font.FontFamilyNumbering = XLFontFamilyNumberingValues.Roman;
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                string dir = localFolder.Path + "\\Reposiciones.xlsx";

                var md = new MessageDialog("", "Mensaje del Sistema");
                try
                {
                    workbook.SaveAs(dir);
                    md.Content = "Se creo el archivo correctamente!";
                    md.ShowAsync();
                }
                catch (Exception e)
                {
                    md.Content = "Hubo un error al crear el archivo: " + e.Message;
                    md.ShowAsync();
                }

            }
        }
        private async Task EntradaPorReposicion()
        {
            var alerta = new MessageDialog("", "Mensaje del Sistema");
            if (FacturaSeleccionada == null)
            {
                alerta.Content = "Selecciona la factura de donde se repone!";
                await alerta.ShowAsync();
            }
            else if (ArticuloSeleccionado == null)
            {
                alerta.Content = "Selecciona el articulo que te repusieron!";
                await alerta.ShowAsync();
            }
            else if (CajaSeleccionada == null)
            {
                alerta.Content = "Selecciona la caja que te repusieron!";
                await alerta.ShowAsync();
            }
            else if (CajasAReponer == 0 && UnidadesAReponer == 0)
            {
                alerta.Content = "La reposicion debe ser por lo menos mayor a una unidad!";
                await alerta.ShowAsync();
            }
            else
            {


                var cajaSelec = CajaSeleccionada as EntradaAlmacen;
                var caja = CajasDisp.Where(b => b.Capacidad == cajaSelec.UnidadesXCaja && b.PrecioXCaja == cajaSelec.PrecioXCaja).FirstOrDefault();
                var division = UnidadesAReponer / cajaSelec.UnidadesXCaja;
                var stockIndACajas = Math.Floor(division);
                UnidadesAReponer = UnidadesAReponer - (stockIndACajas * caja.Capacidad);
                CajasAReponer += (int)stockIndACajas;

                var nuevaEntrada = new EntradaAlmacen();
                nuevaEntrada.NoFactura = cajaSelec.NoFactura;
                nuevaEntrada.Fecha = DateTime.Now.ToShortDateString();
                nuevaEntrada.Articulo = cajaSelec.Articulo;
                nuevaEntrada.CostoTotal = 0;
                nuevaEntrada.PrecioUnitario = 0;
                nuevaEntrada.PrecioUnitarioSinIVA = 0;
                nuevaEntrada.PrecioXCaja = 0;
                nuevaEntrada.PrecioXCajaSinIva = 0;
                nuevaEntrada.Caja = caja.Id.ToString(); ;
                nuevaEntrada.CajasStock += CajasAReponer;
                nuevaEntrada.UnidadesXCaja = caja.Capacidad;
                nuevaEntrada.StockIndividual += UnidadesAReponer;
                if (nuevaEntrada.StockIndividual >= caja.Capacidad)
                {
                    var aCajas = nuevaEntrada.StockIndividual / caja.Capacidad;
                    var numCajas = Math.Floor(aCajas);
                    nuevaEntrada.CajasStock += (int)numCajas;
                    var nuevoStockInd = nuevaEntrada.StockIndividual - (numCajas * caja.Capacidad);
                    nuevaEntrada.StockIndividual = nuevoStockInd;
                }
                nuevaEntrada.Departamento = cajaSelec.Departamento;
                nuevaEntrada.IdDepartamento = cajaSelec.IdDepartamento;
                nuevaEntrada.Cantidad = nuevaEntrada.StockIndividual + (nuevaEntrada.CajasStock * nuevaEntrada.UnidadesXCaja);
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                nuevaEntrada.NoEmpleado = (int)localSettings.Values["NoEmpleado"];
                nuevaEntrada.NombreEmpleado = (string)localSettings.Values["Nombre"] + " " + (string)localSettings.Values["ApPat"] + " " + (string)localSettings.Values["ApMat"];
                nuevaEntrada.IdProveedor = cajaSelec.IdProveedor;
                nuevaEntrada.NombreProveedor = cajaSelec.NombreProveedor;
                nuevaEntrada.ClaveArticulo = cajaSelec.ClaveArticulo;

                caja.CajasDisponibles += nuevaEntrada.CajasStock;
                caja.StockTotal += nuevaEntrada.Cantidad;
                caja.StockIndividual += nuevaEntrada.StockIndividual;
                if (caja.StockIndividual >= caja.Capacidad)
                {
                    var aCajas = caja.StockIndividual / caja.Capacidad;
                    var numCajas = Math.Floor(aCajas);
                    caja.CajasDisponibles += (int)numCajas;
                    var nuevoStockInd = caja.StockIndividual - (numCajas * caja.Capacidad);
                    caja.StockIndividual = nuevoStockInd;
                }
                var contenido = new StringContent(JsonSerializer.Serialize(nuevaEntrada));
                contenido.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(urlEntradas + "AgregarEntrada/", contenido);
                if (response.IsSuccessStatusCode)
                {
                    var content = new StringContent(JsonSerializer.Serialize(caja));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(urlCajas + "UpdateCaja", content);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var md = new MessageDialog("Se agrego el inventario de reposicion!", "Mensaje del Sistema");
                        await md.ShowAsync();
                    }
                    else
                    {
                        var md = new MessageDialog("Hubo un error al agregar la entrada: " + await response.Content.ReadAsStringAsync(), "Mensaje del Sistema");
                        await md.ShowAsync();
                    }
                }
                else
                {
                    var md = new MessageDialog("Hubo un error al agregar la entrada: " + await response.Content.ReadAsStringAsync(), "Mensaje del Sistema");
                    await md.ShowAsync();
                }
            }



        }
        private async void SalidasExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("SALIDAS DE ALMACEN");
                var cont = 4;
                double totalDeTotales = 0;

                foreach (var s in SalidasAlmacen)
                {

                    worksheet.Cell("A1").Value = "Salidas de almacen desde: " + DesdeSalida + " hasta: " + HastaSalida;
                    worksheet.Cell("A3").Value = "Clave del articulo";
                    worksheet.Cell("B3").Value = "Descripcion";
                    worksheet.Cell("C3").Value = "Razon de la salida";
                    worksheet.Cell("D3").Value = "Cajas";
                    worksheet.Cell("E3").Value = "Unidades";
                    worksheet.Cell("F3").Value = "Egreso";
                    worksheet.Cell("G3").Value = "Fecha salida";
                    worksheet.Cell("A" + cont).Value = s.ClaveArticulo;
                    worksheet.Cell("B" + cont).Value = s.NombreArticulo;
                    worksheet.Cell("C" + cont).Value = s.Razon;
                    worksheet.Cell("D" + cont).Value = s.CajasSacadas;
                    worksheet.Cell("E" + cont).Value = s.UnidadesSacadas;
                    worksheet.Cell("F" + cont).Value = s.TotalPerdida;
                    worksheet.Cell("G" + cont).Value = s.FechaSalida;
                    cont++;
                    totalDeTotales += s.TotalPerdida;
                    if (cont == SalidasAlmacen.Count)
                    {
                        worksheet.Cell("F" + (cont + 2).ToString()).Value = "Total de egreso: " + totalDeTotales;
                    }
                }
                worksheet.Column(1).Width = 50;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 30;
                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 30;
                worksheet.Column(7).Width = 30;
                worksheet.Column(7).Width = 50;
                worksheet.Style.Font.SetFontName("Arial");
                worksheet.Style.Font.SetFontSize(12);
                worksheet.Style.Font.FontFamilyNumbering = XLFontFamilyNumberingValues.Roman;
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                string dir = localFolder.Path + "\\SalidasAlmacen.xlsx";
                workbook.SaveAs(dir);

            }
        }
        private async void ExcelCreated()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ENTRADAS DE ALMACEN");
                var cont = 1;
                double totalDeTotales = 0;
                var contFac = 0;
                foreach (var c in ComprasAlmacen)
                {
                    worksheet.Cell("A" + (cont + 1).ToString()).Value = "NoFactura: " + c.Key.NoFactura;
                    worksheet.Cell("B" + (cont + 1).ToString()).Value = "Empleado que registro: " + c.Key.NombreEmpleado;
                    worksheet.Cell("C" + (cont + 1).ToString()).Value = "Fecha de entrada: " + c.Key.Fecha;
                    worksheet.Cell("D" + (cont + 1).ToString()).Value = "Proveedor: " + c.Key.NombreProveedor;

                    cont++;
                    worksheet.Cell("A" + (cont + 1).ToString()).Value = "Articulo";
                    worksheet.Cell("B" + (cont + 1).ToString()).Value = "Cajas compradas";
                    worksheet.Cell("C" + (cont + 1).ToString()).Value = "Unidades por caja";
                    worksheet.Cell("D" + (cont + 1).ToString()).Value = "Precio por caja";
                    worksheet.Cell("E" + (cont + 1).ToString()).Value = "Subtotal";
                    cont++;
                    var contLocal = 0;
                    double totalFactura = 0;
                    foreach (var l in c.Value)
                    {
                        worksheet.Cell("A" + (cont + 1).ToString()).Value = l.Articulo;
                        worksheet.Cell("B" + (cont + 1).ToString()).Value = l.CajasStock;
                        worksheet.Cell("C" + (cont + 1).ToString()).Value = l.UnidadesXCaja;
                        worksheet.Cell("D" + (cont + 1).ToString()).Value = l.PrecioXCaja;
                        worksheet.Cell("E" + (cont + 1).ToString()).Value = l.CostoTotal;
                        totalFactura += l.CostoTotal;
                        cont++;
                        contLocal++;

                        if (contLocal == c.Value.Count)
                        {
                            worksheet.Cell("F" + (cont + 1).ToString()).Value = "Total: " + totalFactura;
                            totalDeTotales += totalFactura;
                        }
                    }
                    cont++;
                    contFac++;
                    if (contFac == ComprasAlmacen.Count)
                    {
                        worksheet.Cell("F" + (cont + 2).ToString()).Value = "Total gastado: " + totalDeTotales;
                    }

                }
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 30;
                worksheet.Column(4).Width = 30;

                worksheet.Column(5).Width = 30;
                worksheet.Column(6).Width = 30;
                worksheet.Column(7).Width = 30;
                worksheet.Style.Font.SetFontName("Arial");
                worksheet.Style.Font.SetFontSize(12);
                worksheet.Style.Font.FontFamilyNumbering = XLFontFamilyNumberingValues.Roman;
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                string dir = localFolder.Path + "\\EntradasAlmacen.xlsx";
                workbook.SaveAs(dir);

            }
        }
        private void QuitarEntrada(EntradaAlmacen obj)
        {
            var entrada = obj as EntradaAlmacen;
            TotalCompra -= entrada.CostoTotal;


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
                        if (!PorCaja)
                            articulo.Stock = (float)Cantidad + stock;
                        articulo.PrecioAlCosto = PrecioUnitario;
                        articulo.IdProveedor = entrada.IdProveedor;
                        articulo.NombreProveedor = entrada.NombreProveedor;
                        articulo.StockIndividual = 0;
                        if (PorCaja)
                        {
                            articulo.Caja = "Si";
                            articulo.CajasStock += (int)CantidadCajasCompradas;

                            articulo.UnidadesXCaja = UnidadXCajas;
                            articulo.Stock = (float)(UnidadXCajas * CantidadCajasCompradas) + articulo.Stock + (float)entrada.StockIndividual;
                            articulo.StockIndividual = StockIndividual;
                            articulo.PrecioXCaja = PrecioXCaja;
                            var caja = new CajaDispArticulo();
                            caja.CajasDisponibles = entrada.CajasStock;
                            caja.StockTotal = (float)(entrada.UnidadesXCaja * entrada.CajasStock) + caja.StockTotal + (float)entrada.StockIndividual;
                            caja.Capacidad = entrada.UnidadesXCaja;
                            caja.ClaveArticulo = entrada.ClaveArticulo;
                            caja.PrecioUnitarioArt = Math.Round(entrada.PrecioUnitario, 2);
                            caja.PrecioXCaja = entrada.PrecioXCaja;
                            caja.StockIndividual = entrada.StockIndividual;
                            var compra = new CompraAProveedor();
                            compra.CajasDisponibles = entrada.CajasStock;
                            compra.Capacidad = entrada.UnidadesXCaja;
                            compra.ClaveArticulo = entrada.ClaveArticulo;
                            compra.FechaEntrada = DateValue;
                            compra.IdProveedor = entrada.IdProveedor;
                            compra.NombreProveedor = entrada.NombreProveedor;
                            compra.PrecioUnitarioArt = entrada.PrecioUnitario;
                            compra.StockIndividual = entrada.StockIndividual;
                            compra.StockTotal = (float)(entrada.UnidadesXCaja * entrada.CajasStock) + (float)entrada.StockIndividual;
                            compra.SubTotal = Math.Round(((float)(entrada.UnidadesXCaja * entrada.CajasStock) + entrada.StockIndividual) * entrada.PrecioUnitario, 2);
                            var c = new StringContent(JsonSerializer.Serialize(caja), Encoding.UTF8);
                            c.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            responseMessage = await httpClient.PostAsync(urlCajas + "AgregarCajaDisp", c);
                            if (responseMessage.IsSuccessStatusCode)
                            {
                                c = new StringContent(JsonSerializer.Serialize(compra), Encoding.UTF8);
                                c.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                                responseMessage = await httpClient.PostAsync(urlCompras + "AgregarCompraProv", c);
                                if (!responseMessage.IsSuccessStatusCode)
                                {
                                    var m = new MessageDialog("Hubo un problema al agregar la entrada!", "Error");
                                }
                            }


                        }
                        else
                        {
                            var caja = new CajaDispArticulo();
                            caja.CajasDisponibles = entrada.CajasStock;
                            caja.StockTotal = (float)(entrada.UnidadesXCaja * entrada.CajasStock) + caja.StockTotal + (float)entrada.StockIndividual;
                            caja.Capacidad = entrada.UnidadesXCaja;
                            caja.ClaveArticulo = entrada.ClaveArticulo;
                            caja.PrecioUnitarioArt = Math.Round(entrada.PrecioUnitario, 2);
                            caja.PrecioXCaja = entrada.PrecioXCaja;
                            caja.StockIndividual = entrada.StockIndividual;
                            var c = new StringContent(JsonSerializer.Serialize(caja), Encoding.UTF8);
                            c.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            responseMessage = await httpClient.PostAsync(urlCajas + "AgregarCajaDisp", c);
                        }
                        content = new StringContent(JsonSerializer.Serialize(articulo), Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        HttpResponseMessage respuesta = await httpClient.PostAsync(urlArticulos + "UpdateArticulo/", content);
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
                StockIndividualAEntrar = 0;
                TotalCompra = 0;
                CantidadCajasCompradas = 0;
                PrecioUnitario = 0;
                PrecioXCaja = 0;
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
            var alerta = new MessageDialog("", "Mensaje del Sistema");
            if (NoFactura == null)
            {
                alerta.Content = "Agrega el numero de factura de la compra!";
                alerta.ShowAsync();
            }
            else if (ComboDepartamento == null)
            {
                alerta.Content = "Selecciona el departamento!";
                alerta.ShowAsync();
            }
            else if (ComboArticulo == null)
            {
                alerta.Content = "Selecciona el articulo que compraste!";
                alerta.ShowAsync();
            }
            else if (ComboProveedor == null)
            {
                alerta.Content = "Selecciona al proveedor que te vendio!";
                alerta.ShowAsync();
            }
            else if (Cantidad == 0)
            {
                alerta.Content = "Agrega por lo menos una pieza!";
                alerta.ShowAsync();
            }
            else
            {
                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                var entrada = new EntradaAlmacen();
                var entradaGrid = new EntradaGrid();
                entrada.Articulo = (ComboArticulo as Articulo).Descripcion;
                entradaGrid.Articulo = (ComboArticulo as Articulo).Descripcion;
                entrada.Cantidad = Cantidad;
                entradaGrid.Cantidad = Cantidad;
                entrada.ClaveArticulo = (ComboArticulo as Articulo).Clave;
                entradaGrid.ClaveArticulo = (ComboArticulo as Articulo).Clave;
                entrada.CostoTotal = Math.Round(CostoTotal, 2);
                entradaGrid.CostoTotal = CostoTotal;
                entrada.Departamento = (ComboDepartamento as Departamento).Nombre;
                entradaGrid.Departamento = (ComboDepartamento as Departamento).Nombre;
                entrada.NombreMoneda = (ComboMoneda as Moneda).Nombre;
                entradaGrid.NombreMoneda = (ComboMoneda as Moneda).Nombre;
                entrada.IdMoneda = (ComboMoneda as Moneda).Id;
                entradaGrid.IdMoneda = (ComboMoneda as Moneda).Id;
                entrada.IdProveedor = (ComboProveedor as Proveedor).Id;
                entradaGrid.IdProveedor = (ComboProveedor as Proveedor).Id;
                entrada.NombreProveedor = (ComboProveedor as Proveedor).Nombre;
                entradaGrid.NombreProveedor = (ComboProveedor as Proveedor).Nombre;
                entrada.PrecioUnitario = Math.Round(PrecioUnitario, 2);
                entradaGrid.PrecioUnitario = PrecioUnitario;
                entrada.IdDepartamento = (ComboDepartamento as Departamento).Id;
                entradaGrid.IdDepartamento = (ComboDepartamento as Departamento).Id;
                // entrada.CajasStock = (int)CantidadCajasCompradas;
                //entrada.UnidadesXCaja = UnidadXCajas;
                entrada.Fecha = DateValue;
                entradaGrid.Fecha = DateValue;
                entrada.Caja = PorCaja ? "Si" : "No";

                if (PorCaja)
                {
                    entrada.CajasStock = (int)CantidadCajasCompradas;
                    entrada.UnidadesXCaja = UnidadXCajas;
                    entrada.PrecioXCaja = PrecioXCaja;
                    entradaGrid.PrecioXCaja = PrecioXCaja;
                    entrada.StockIndividual = StockIndividualAEntrar;

                }
                else
                {
                    entrada.CajasStock = 0;
                    entrada.UnidadesXCaja = 1;
                    entrada.StockIndividual = StockIndividualAEntrar;
                    entrada.PrecioXCaja = PrecioXCaja;
                }

                entrada.NoFactura = NoFactura;
                entradaGrid.NoFactura = NoFactura;
                entrada.NoEmpleado = (int)localSettings.Values["NoEmpleado"];
                entrada.NombreEmpleado = (string)localSettings.Values["Nombre"] + " " + (string)localSettings.Values["ApPat"] + " " + (string)localSettings.Values["ApMat"];
                TotalCompra += CostoTotal;
                listaTemporal.Add(entrada);
                entradaTabla.Add(entradaGrid);
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

}
