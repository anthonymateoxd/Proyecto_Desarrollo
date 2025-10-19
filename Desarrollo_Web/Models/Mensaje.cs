using System.ComponentModel.DataAnnotations;

namespace DemonSlayer.Models
{
    public class Mensaje
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Texto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public int ProyectoId { get; set; }
        public bool Leido { get; set; }

        public int RemitenteId { get; set; }

        // DESTINATARIO (NUEVO)
        public int DestinatarioId { get; set; }
    }
}
