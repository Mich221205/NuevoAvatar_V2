namespace PV_NA_Matricula.Entities
{
    public class ExpedienteMovilUpdate
    {
        public string Telefono { get; set; } = string.Empty;

        public int IdProvincia { get; set; }
        public int IdCanton { get; set; }
        public int IdDistrito { get; set; }

        public string DetalleDireccion { get; set; } = string.Empty;
    }
}
