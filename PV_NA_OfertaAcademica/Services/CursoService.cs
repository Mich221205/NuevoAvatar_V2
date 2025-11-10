using PV_NA_OfertaAcademica.Dtos;
using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Helpers;
using PV_NA_OfertaAcademica.Repository;
using System.Text.Json;
using System.Net.Http.Json;

namespace PV_NA_OfertaAcademica.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio para cursos.
    /// Incluye validaciones, control de integridad y registro de bitácoras (GEN1).
    /// </summary>
    public interface ICursoService
    {
        Task<int> Crear(CursoCreateDto dto, string usuario);
        Task Actualizar(int id, CursoUpdateDto dto, string usuario);
        Task Eliminar(int id, string usuario);
        Task<CursoReadDto?> Obtener(int id, string usuario);
        Task<IEnumerable<CursoReadDto>> Listar(string usuario);
        Task<IEnumerable<CursoReadDto>> ListarPorCarrera(int idCarrera, string usuario);
    }

    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public CursoService(ICursoRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        //  Crear curso con validaciones y bitácora
        public async Task<int> Crear(CursoCreateDto dto, string usuario)
        {
            if (!ValidationUtils.NombreValido(dto.Nombre))
                throw new ArgumentException("El nombre solo permite letras/espacios y máx. 100 caracteres.");
            if (!ValidationUtils.NivelValido(dto.Nivel))
                throw new ArgumentException("Nivel debe estar entre 1 y 12.");
            if (!await _repo.CarreraExistsAsync(dto.ID_Carrera))
                throw new ArgumentException("La carrera no existe.");
            if (await _repo.ExistsByNombreAsync(dto.ID_Carrera, dto.Nombre.Trim()))
                throw new InvalidOperationException("Ya existe un curso con ese nombre en la carrera.");

            var curso = new Curso
            {
                Nombre = dto.Nombre.Trim(),
                Nivel = dto.Nivel,
                ID_Carrera = dto.ID_Carrera
            };

            var id = await _repo.CreateAsync(curso);

            await RegistrarBitacoraAsync(usuario, $"Creó curso: {JsonSerializer.Serialize(curso)}");
            return id;
        }

        //  Actualizar curso con validaciones y bitácora
        public async Task Actualizar(int id, CursoUpdateDto dto, string usuario)
        {
            var actual = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Curso no encontrado.");

            if (!ValidationUtils.NombreValido(dto.Nombre))
                throw new ArgumentException("El nombre solo permite letras/espacios y máx. 100 caracteres.");
            if (!ValidationUtils.NivelValido(dto.Nivel))
                throw new ArgumentException("Nivel debe estar entre 1 y 12.");
            if (!await _repo.CarreraExistsAsync(dto.ID_Carrera))
                throw new ArgumentException("La carrera no existe.");
            if (await _repo.ExistsByNombreAsync(dto.ID_Carrera, dto.Nombre.Trim(), id))
                throw new InvalidOperationException("Ya existe un curso con ese nombre en la carrera.");

            var anterior = JsonSerializer.Serialize(actual);

            actual.Nombre = dto.Nombre.Trim();
            actual.Nivel = dto.Nivel;
            actual.ID_Carrera = dto.ID_Carrera;

            await _repo.UpdateAsync(actual);
            await RegistrarBitacoraAsync(usuario, $"Actualizó curso: Antes={anterior}, Ahora={JsonSerializer.Serialize(actual)}");
        }

        //  Eliminar curso con validación de dependencias y bitácora
        public async Task Eliminar(int id, string usuario)
        {
            var actual = await _repo.GetByIdAsync(id) ?? throw new KeyNotFoundException("Curso no encontrado.");
            if (await _repo.HasGruposAsync(id))
                throw new InvalidOperationException("No se puede eliminar: existen grupos asociados.");

            await _repo.DeleteAsync(id);
            await RegistrarBitacoraAsync(usuario, $"Eliminó curso: {JsonSerializer.Serialize(actual)}");
        }

        //  Obtener curso con registro de consulta
        public async Task<CursoReadDto?> Obtener(int id, string usuario)
        {
            var c = await _repo.GetByIdAsync(id);
            await RegistrarBitacoraAsync(usuario, $"Consultó curso con ID {id}");
            return c is null ? null : new CursoReadDto(c.ID_Curso, c.Nombre, c.Nivel, c.ID_Carrera);
        }

        //  Listar cursos
        public async Task<IEnumerable<CursoReadDto>> Listar(string usuario)
        {
            var cursos = await _repo.GetAllAsync();
            await RegistrarBitacoraAsync(usuario, "Consultó listado de cursos");
            return cursos.Select(c => new CursoReadDto(c.ID_Curso, c.Nombre, c.Nivel, c.ID_Carrera));
        }

        // Listar cursos por carrera
        public async Task<IEnumerable<CursoReadDto>> ListarPorCarrera(int idCarrera, string usuario)
        {
            if (!await _repo.CarreraExistsAsync(idCarrera))
                throw new ArgumentException("La carrera no existe.");

            var cursos = await _repo.GetByCarreraAsync(idCarrera);
            await RegistrarBitacoraAsync(usuario, $"Consultó cursos por carrera ID {idCarrera}");
            return cursos.Select(c => new CursoReadDto(c.ID_Curso, c.Nombre, c.Nivel, c.ID_Carrera));
        }

        // Método auxiliar para registrar en Bitácora (GEN1)
        private async Task RegistrarBitacoraAsync(string usuario, string accion)
        {
            try
            {
                await _bitacoraClient.PostAsJsonAsync("/bitacora", new
                {
                    ID_Usuario = usuario,
                    Accion = accion
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error al registrar bitácora: {ex.Message}");
            }
        }
    }
}
