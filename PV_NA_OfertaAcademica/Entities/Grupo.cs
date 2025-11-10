namespace PV_NA_OfertaAcademica.Entities
{
    public class Grupo
    {
        public int ID_Grupo { get; set; }
        public int Numero_Grupo { get; set; }
        public int ID_Curso { get; set; }
        public int ID_Profesor { get; set; }
        public string Horario { get; set; } = string.Empty;
        public int ID_Periodo { get; set; }
    }
}

