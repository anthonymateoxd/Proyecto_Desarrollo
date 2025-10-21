using System.ComponentModel.DataAnnotations;

namespace DemonSlayer.Models
{
    public class Mensaje
    {
        public int Id { get; set; }

        [Required]
        public string Texto { get; set; } = string.Empty; // Agrega valor por defecto

        // CAMBIA ESTO: Especifica el Kind como UTC
        public DateTime Fecha { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);


        public int ProyectoId { get; set; }
        public bool Leido { get; set; }

        public int RemitenteId { get; set; }

        // DESTINATARIO (NUEVO)
        public int DestinatarioId { get; set; }
    }
}
