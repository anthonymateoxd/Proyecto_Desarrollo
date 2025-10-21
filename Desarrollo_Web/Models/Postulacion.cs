using System.ComponentModel.DataAnnotations;

namespace DemonSlayer.Models
{
    public class Postulacion
    {
        public int Id { get; set; }

        public decimal MontoQ { get; set; }

        [MaxLength(1000)]
        public string Propuesta { get; set; }

        // CAMBIA ESTO: Especifica el Kind como UTC
        public DateTime Fecha { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);


        public int ProyectoId { get; set; }

        public int ConsultorId { get; set; }
        /*
        public bool Aceptada { get; set; } = false;
        */
        public string Estado { get; set; } = "Pendiente"; // "Pendiente", "Aceptada", "Rechazada"

    }
}
