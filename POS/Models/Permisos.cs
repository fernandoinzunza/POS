using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Permisos
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public bool NuevoArticulo { get; set; }
        public bool ModificarArticulo { get; set; }
        public bool EliminarArticulo { get; set; }
        public bool NuevoDescuento { get; set; }
        public bool NuevaEntrada { get; set; }
        public bool EntradaPorReposicion { get; set; }
        public bool BitacoraEntradas { get; set; }
        public bool StockDisponible { get; set; }
        public bool NuevaSalida { get; set; }
        public bool BitacoraDeSalidas { get; set; }
        public bool NuevoProveedor { get; set; }
        public bool EliminarProveedor { get; set; }
        public bool EditarProveedor { get; set; }
        public bool VentanaDeCobro { get; set; }
        public bool RealizarCancelacion { get; set; }
        public bool ReporteDeDevoluciones { get; set; }
        public bool ReporteDeVentas { get; set; }
        public bool ReporteMasVendidos { get; set; }

    }
}
