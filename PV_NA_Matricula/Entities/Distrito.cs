namespace PV_NA_Matricula.Entities
{
    public class Distrito
    {
        public int ID_Distrito { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int ID_Canton { get; set; }
        public int ID_Provincia { get; set; } 
    }
}
