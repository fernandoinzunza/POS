using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS.Models
{
   public class DescuentosPermitidos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPermiso
        {
            get; set;
        }
        public int NoEmpleado
        {
            get;set;
        }
        public int IdDescuento
        {
            get;set;
        }
    }
}
