namespace PV_NA_OfertaAcademica.Entities
{
    public class Profesor
    {
        public int ID_Profesor { get; set; }
        public string Identificacion { get; set; }
        public string Tipo_Identificacion { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
    }
}

