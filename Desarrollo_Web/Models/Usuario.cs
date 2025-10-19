using System.ComponentModel.DataAnnotations;

namespace DemonSlayer.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreCompleto { get; set; }

        [Required]
        [MaxLength(50)]
        public string Correo { get; set; }

        [Required]
        [MaxLength(100)]
        public string Clave { get; set; }

        [Required] // MANTENEMOS el Required
        [MaxLength(20)]
        public string Rol { get; set; } 

    }
}
