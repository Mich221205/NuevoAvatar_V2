namespace PV_NA_Matricula.Entities
{
    public class Bitacora
    {
        public int ID_Bitacora { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
