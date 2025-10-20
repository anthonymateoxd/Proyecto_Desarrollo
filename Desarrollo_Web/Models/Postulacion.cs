using System.ComponentModel.DataAnnotations;

namespace DemonSlayer.Models
{
    public class Postulacion
    {
        public int Id { get; set; }

        public decimal MontoQ { get; set; }

        [MaxLength(1000)]
        public string Propuesta { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public int ProyectoId { get; set; }

        public int ConsultorId { get; set; }
        /*
        public bool Aceptada { get; set; } = false;
        */
        public string Estado { get; set; } = "Pendiente"; // "Pendiente", "Aceptada", "Rechazada"

    }
}
