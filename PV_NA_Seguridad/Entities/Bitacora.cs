namespace PV_NA_Seguridad.Entities
{
    public class Bitacora
    {
        public int ID_Bitacora { get; set; }
        public int ID_Usuario { get; set; }
        public string Accion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
