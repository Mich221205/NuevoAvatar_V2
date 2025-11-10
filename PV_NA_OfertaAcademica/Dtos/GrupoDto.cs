namespace PV_NA_OfertaAcademica.Dtos
{

    // DTO para crear un nuevo grupo
    public class GrupoCreateDto
    {
        public int Numero_Grupo { get; set; }
        public int ID_Curso { get; set; }
        public int ID_Profesor { get; set; }
        public string Horario { get; set; } = string.Empty;
        public int ID_Periodo { get; set; }
    }
    // DTO para actualizar un grupo, incluye el ID del grupo
    public class GrupoUpdateDto : GrupoCreateDto
    {
        public int ID_Grupo { get; set; }
    }
}
