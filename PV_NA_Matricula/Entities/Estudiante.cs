namespace PV_NA_Matricula.Entities
{
    public class Estudiante
    {
        public int ID_Estudiante { get; set; }
        public string Identificacion { get; set; } = string.Empty;
		public string Tipo_Identificacion { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public DateTime Fecha_Nacimiento { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}
