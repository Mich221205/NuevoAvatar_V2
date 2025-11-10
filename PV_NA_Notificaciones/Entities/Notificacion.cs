namespace PV_NA_Notificaciones.Entities
{
    public class Notificacion
    {
        public int ID_Notificacion { get; set; }
        public string Email_Destino { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string Cuerpo { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
