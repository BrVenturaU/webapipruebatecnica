using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIPruebaTecnica.Models
{
    public class ClienteDTO
    {
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
        public DateTime FechaCreacion { get; set; }
        [Required]
        [StringLength(20)]
        public string CodUsuarioActualizacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public List<CuentaDTO> CuentasCliente { get; set; }
    }
}
