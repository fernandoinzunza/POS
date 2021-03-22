using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class CompraAProveedor
    {
        public int Id { get; set; }
        public double Capacidad { get; set; }
        public string ClaveArticulo { get; set; }
        public int CajasDisponibles { get; set; }
        public double PrecioXCaja { get; set; }
        public double PrecioUnitarioArt { get; set; }
        public double StockIndividual { get; set; }
        public double StockTotal { get; set; }
        public string FechaEntrada { get; set; }
        public int IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public double SubTotal { get; set; }
        public double PrecioUnitarioSinIVA
        {
            get;set;
        }
        public double PrecioXCajaSinIVA
        {
            get;set;
        }
        public double SubTotalSinIVA
        {
            get;set;
        }
    }
}
