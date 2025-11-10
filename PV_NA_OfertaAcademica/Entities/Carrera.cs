namespace PV_NA_OfertaAcademica.Entities
{
    public class Carrera
    {
        public int ID_Carrera { get; set; }
        public string Nombre { get; set; }
        public int ID_Institucion { get; set; }
        public int ID_Profesor_Director { get; set; }
    }
}

