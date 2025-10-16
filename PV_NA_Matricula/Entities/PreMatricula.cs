namespace PV_NA_Matricula.Entities
{
    public class PreMatricula
    {
        public int ID_Prematricula { get; set; }
        public int ID_Estudiante { get; set; }
        public int ID_Carrera { get; set; }
        public int ID_Curso { get; set; }
        public string? Observaciones { get; set; }
        public int ID_Periodo { get; set; } 
    }
}

