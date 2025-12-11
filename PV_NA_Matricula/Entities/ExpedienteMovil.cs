namespace PV_NA_Matricula.Entities
{
    public class ExpedienteMovilDto
    {
        public int IdEstudiante { get; set; }

        public string Identificacion { get; set; } = string.Empty;
        public string TipoIdentificacion { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string DireccionActual { get; set; } = string.Empty;
    }
}
