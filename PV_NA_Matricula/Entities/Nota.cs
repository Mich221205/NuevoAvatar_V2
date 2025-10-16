namespace PV_NA_Matricula.Entities
{
    public class Nota
    {
        public int ID_Nota { get; set; }
        public int ID_Rubro { get; set; }
        public int ID_Estudiante { get; set; }
        public decimal Valor { get; set; }
        public int ID_Grupo { get; set; } 
	}
}
