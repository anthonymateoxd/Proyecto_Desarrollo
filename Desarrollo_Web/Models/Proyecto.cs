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

        // CAMBIA ESTO: Especifica el Kind como UTC o Unspecified
        public DateTime FechaLimite { get; set; } = DateTime.SpecifyKind(DateTime.Now.AddDays(30), DateTimeKind.Utc);

        [MaxLength(20)]
        public string Estado { get; set; } = "Activo";

        public int ClienteId { get; set; }
    }
}
