namespace PV_NA_OfertaAcademica.Dtos
{
    // crear periodo
    public class PeriodoCreateDto
    {
        public int Anio { get; set; }
        public int Numero_Periodo { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Fin { get; set; }
    }

    // actualizar periodo
    public class PeriodoUpdateDto : PeriodoCreateDto
    {
        public int ID_Periodo { get; set; }
    }
}
