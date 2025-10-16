namespace PV_NA_Matricula.Entities
{
    public class NotaRubro
    {
        public int ID_Rubro { get; set; }
        public int ID_Grupo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Porcentaje { get; set; }
    }
}

