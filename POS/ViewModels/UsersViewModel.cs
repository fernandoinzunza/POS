using POS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace POS.ViewModels
{
    public class UsersViewModel : BaseViewModel
    {
        private bool nuevoArticulo;
        private bool modificarArticulo;
        private bool eliminarArticulo;
        private bool nuevoDescuento;
        private bool nuevaEntrada;
        private bool entradaPorReposicion;
        private bool bitacoraEntradas;
        private bool stockDisponible;
        private bool nuevaSalida;
        private bool bitacoraDeSalidas;
        private bool nuevoProveedor;
        private bool eliminarProveedor;
        private bool editarProveedor;
        private bool ventanaDeCobro;
        private bool realizarCancelacion;
        private bool reporteDeDevoluciones;
        private bool reporteDeVentas;
        private bool reporteMasVendidos;
        private string puesto;
        private string nombre;
        private string apPat;
        private string apMat;
        private string nivel;
        private string contrasena;
        private string usuario;
        private const string urlUsuarios = "http://localhost:9095/api/Usuarios/";
        private const string urlPermisos = "http://localhost:9095/api/PermisosDisps/";
        private HttpClient httpClient;
        private HttpResponseMessage response;
        private ObservableCollection<Usuario> usuarios;
        public string Usuario { get { return usuario; } set { SetProperty(ref usuario, value); } }
        public string Puesto { get { return puesto; } set { SetProperty(ref puesto, value); } }
        public string Nombre { get { return nombre; } set { SetProperty(ref nombre, value); } }
        public string ApPat { get { return apPat; } set { SetProperty(ref apPat, value); } }
        public string ApMat { get { return apMat; } set { SetProperty(ref apMat, value); } }
        public string Nivel { get { return nivel; } set { SetProperty(ref nivel, value); } }
        public string Contrasena { get { return contrasena; } set { SetProperty(ref contrasena, value); } }
        public bool NuevoArticulo { get { return nuevoArticulo; } set { SetProperty(ref nuevoArticulo, value); } }
        public bool ModificarArticulo { get { return modificarArticulo; } set { SetProperty(ref modificarArticulo, value); } }
        public bool EliminarArticulo { get { return eliminarArticulo; } set { SetProperty(ref eliminarArticulo, value); } }
        public bool NuevoDescuento { get { return nuevoDescuento; } set { SetProperty(ref nuevoDescuento, value); } }
        public bool NuevaEntrada { get { return nuevaEntrada; } set { SetProperty(ref nuevaEntrada, value); } }
        public bool EntradaPorReposicion { get { return entradaPorReposicion; } set { SetProperty(ref entradaPorReposicion, value); } }
        public bool BitacoraEntradas { get { return bitacoraEntradas; } set { SetProperty(ref bitacoraEntradas, value); } }
        public bool StockDisponible { get { return stockDisponible; } set { SetProperty(ref stockDisponible, value); } }
        public bool NuevaSalida { get { return nuevaSalida; } set { SetProperty(ref nuevaSalida, value); } }
        public bool BitacoraDeSalidas { get { return bitacoraDeSalidas; } set { SetProperty(ref bitacoraDeSalidas, value); } }
        public bool NuevoProveedor { get { return nuevoProveedor; } set { SetProperty(ref nuevoProveedor, value); } }
        public bool EliminarProveedor { get { return eliminarProveedor; } set { SetProperty(ref eliminarProveedor, value); } }
        public bool EditarProveedor { get { return editarProveedor; } set { SetProperty(ref editarProveedor, value); } }
        public bool VentanaDeCobro { get { return ventanaDeCobro; } set { SetProperty(ref ventanaDeCobro, value); } }
        public bool RealizarCancelacion { get { return realizarCancelacion; } set { SetProperty(ref realizarCancelacion, value); } }
        public bool ReporteDeDevoluciones { get { return reporteDeDevoluciones; } set { SetProperty(ref reporteDeDevoluciones, value); } }
        public bool ReporteDeVentas { get { return reporteDeVentas; } set { SetProperty(ref reporteDeVentas, value); } }
        public bool ReporteMasVendidos { get { return reporteMasVendidos; } set { SetProperty(ref reporteMasVendidos, value); } }
        public RelayCommand AgregarUsuario { get; set; }
        public ObservableCollection<Usuario> Usuarios { get { return usuarios; } set { SetProperty(ref usuarios, value); } }
        private object tree;
        public object Tree
        {

            get { return tree; }

            set { SetProperty(ref tree, value); }
        }
        public UsersViewModel()
        {
            httpClient = new HttpClient();
            AgregarUsuario = new RelayCommand(async (o) => { await NuevoUsuarioAsync(); });
            response = httpClient.GetAsync(urlUsuarios + "GetUsuarios").Result;
            if(response.IsSuccessStatusCode)
            {
                Usuarios = new ObservableCollection<Usuario>(JsonSerializer.Deserialize<List<Usuario>>(response.Content.ReadAsStringAsync().Result, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
            }
        }
        public async Task NuevoUsuarioAsync()
        {
            var usuario = new Usuario();
           
            usuario.ApMaterno = ApMat;
            usuario.ApPaterno = ApPat;
            usuario.Contrasena = Contrasena;
            usuario.Puesto = Puesto;
            usuario.Nombre = Nombre;
            usuario.NomUsuario = Usuario;
            var json = JsonSerializer.Serialize(usuario);
            var content = new StringContent(json, Encoding.UTF8);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = await httpClient.PostAsync(urlUsuarios + "NuevoUsuario", content);
            if (response.IsSuccessStatusCode)
            {
                var newUser = JsonSerializer.Deserialize<Usuario>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var permisos = new Permisos();
                permisos.IdUsuario = newUser.NoEmpleado;
                permisos.BitacoraDeSalidas = BitacoraDeSalidas;
                permisos.BitacoraEntradas = BitacoraEntradas;
                permisos.EditarProveedor = EditarProveedor;
                permisos.EliminarArticulo = EliminarArticulo;
                permisos.EliminarProveedor = EliminarProveedor;
                permisos.EntradaPorReposicion = EntradaPorReposicion;
                permisos.ModificarArticulo = ModificarArticulo;
                permisos.NuevaEntrada = NuevaEntrada;
                permisos.NuevaSalida = NuevaSalida;
                permisos.NuevoArticulo = NuevoArticulo;
                permisos.NuevoDescuento = NuevoDescuento;
                permisos.NuevoProveedor = NuevoProveedor;
                permisos.RealizarCancelacion = RealizarCancelacion;
                permisos.ReporteDeDevoluciones = ReporteDeDevoluciones;
                permisos.ReporteDeVentas = ReporteDeVentas;
                permisos.ReporteMasVendidos = ReporteMasVendidos;
                permisos.VentanaDeCobro = VentanaDeCobro;
                permisos.StockDisponible = StockDisponible;

                var jeison = JsonSerializer.Serialize(permisos);
                var contenido = new StringContent(jeison, Encoding.UTF8);
                contenido.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                response = await httpClient.PostAsync(urlPermisos + "NuevoPermiso", contenido);
                if (response.IsSuccessStatusCode)
                {
                    var md = new MessageDialog("Se agrego al nuevo usuario correctamente", "Mensaje del Sistema");
                    await md.ShowAsync();
                }
                else
                {
                    var md = new MessageDialog("Hubo un error " + await response.Content.ReadAsStringAsync());
                   await md.ShowAsync();
                }
            }

        }
    }
}
