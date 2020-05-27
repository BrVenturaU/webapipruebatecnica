using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIPruebaTecnica.Entities
{
    public class Cliente
    {
        [Timestamp]
        public byte[] TimeStamp { get; set; }
        public int Id { get; set; }
        [Required]
        [StringLength(10)]
        public string CodCliente { get; set; }
        [Required]
        [StringLength(100)]
        public string NombreCliente { get; set; }
        [Required]
        [StringLength(20)]
        public string CodRuta { get; set; }
        [Required]
        public bool Activo { get; set; } = true;
        [Required]
        [StringLength(20)]
        public string CodUsuarioCreo { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        [Required]
        [StringLength(20)]
        public string CodUsuarioActualizacion { get; set; }
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
        public List<Cuenta> CuentasCliente { get; set; }
    }
}
