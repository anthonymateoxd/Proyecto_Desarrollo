using System.ComponentModel.DataAnnotations;

namespace DemonSlayer.Models
{
    public class Proyecto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Titulo { get; set; }

        [MaxLength(1000)]
        public string Descripcion { get; set; }

        public decimal PresupuestoQ { get; set; }

        public DateTime FechaLimite { get; set; }

        [MaxLength(20)]
        public string Estado { get; set; } = "Activo";

        public int ClienteId { get; set; }
    }
}
