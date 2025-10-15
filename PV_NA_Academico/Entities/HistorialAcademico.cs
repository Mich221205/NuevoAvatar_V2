namespace PV_NA_Academico.Entities
{
    public class HistorialAcademico
    {
        public string TipoIdentificacion { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public string CodigoCurso { get; set; } = string.Empty;
        public string NombreCurso { get; set; } = string.Empty;
        public decimal Promedio { get; set; }
        public string Periodo { get; set; } = string.Empty;
    }
}
