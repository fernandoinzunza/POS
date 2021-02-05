using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace POS.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoEmpleado
        {
            get; set;
        }
        [Column(TypeName = "varchar(100)")]
        public string Nombre
        {
            get; set;
        }
        [Column(TypeName = "varchar(100)")]
        public string ApPaterno { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string ApMaterno { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Puesto { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string NomUsuario { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Contrasena { get; set; }


    }
}
