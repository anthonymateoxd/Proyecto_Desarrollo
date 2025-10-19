using System.ComponentModel.DataAnnotations;

namespace DemonSlayer.ViewModels
{
    public class UsuarioVM
    {

        [Required(ErrorMessage = "El nombre completo es requerido")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "Confirmar contraseña es requerido")]
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmarClave { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
        [Display(Name = "Tipo de Usuario")]
        public string Rol { get; set; } // "Cliente" o "Consultor"
        // REMITENTE
        public int RemitenteId { get; set; }

        // DESTINATARIO (NUEVO)
        public int DestinatarioId { get; set; }
    }
}
