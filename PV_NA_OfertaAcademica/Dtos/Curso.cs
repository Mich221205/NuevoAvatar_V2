namespace PV_NA_OfertaAcademica.Dtos
{
        public record CursoCreateDto(string Nombre, int Nivel, int ID_Carrera);
        public record CursoUpdateDto(string Nombre, int Nivel, int ID_Carrera);
        public record CursoReadDto(int ID_Curso, string Nombre, int Nivel, int ID_Carrera);
 
}
