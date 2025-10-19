using DemonSlayer.Models;

namespace DemonSlayer.ViewModels
{
    public class MensajeConUsuario
    {
        public Mensaje Mensaje { get; set; }
        public string NombreRemitente { get; set; }
        public bool EsMio { get; set; }
    }
}
