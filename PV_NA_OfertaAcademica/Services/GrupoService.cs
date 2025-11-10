using PV_NA_OfertaAcademica.Dtos;
using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Repository;
using System.Net.Http.Json;
using System.Text.Json;

namespace PV_NA_OfertaAcademica.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio para los Grupos.
    /// Aplica validaciones, control de integridad y registra acciones en Bitácora (GEN1).
    /// </summary>
    public class GrupoService : IGrupoService
    {
        private readonly GrupoRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public GrupoService(GrupoRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        // 🔹 Obtener todos los grupos
        public async Task<IEnumerable<Grupo>> ObtenerTodosAsync(string usuario)
        {
            var grupos = await _repo.GetAllAsync();
            await RegistrarBitacoraAsync(usuario, "Consultó listado de grupos");
            return grupos;
        }

        // 🔹 Obtener grupo por ID
        public async Task<Grupo?> ObtenerPorIdAsync(int id, string usuario)
        {
            var grupo = await _repo.GetByIdAsync(id);
            await RegistrarBitacoraAsync(usuario, $"Consultó grupo con ID {id}");
            return grupo;
        }

        // 🔹 Crear grupo
        public async Task<bool> CrearAsync(GrupoCreateDto dto, string usuario)
        {
            ValidarGrupo(dto);

            var grupo = new Grupo
            {
                Numero_Grupo = dto.Numero_Grupo,
                ID_Curso = dto.ID_Curso,
                ID_Profesor = dto.ID_Profesor,
                Horario = dto.Horario,
                ID_Periodo = dto.ID_Periodo
            };

            var result = await _repo.CreateAsync(grupo) > 0;

            if (result)
                await RegistrarBitacoraAsync(usuario, $"Creó grupo: {JsonSerializer.Serialize(grupo)}");

            return result;
        }

        // 🔹 Modificar grupo
        public async Task<bool> ModificarAsync(GrupoUpdateDto dto, string usuario)
        {
            ValidarGrupo(dto);

            var anterior = await _repo.GetByIdAsync(dto.ID_Grupo);
            if (anterior is null)
                throw new ArgumentException("El grupo especificado no existe.");

            var actualizado = new Grupo
            {
                ID_Grupo = dto.ID_Grupo,
                Numero_Grupo = dto.Numero_Grupo,
                ID_Curso = dto.ID_Curso,
                ID_Profesor = dto.ID_Profesor,
                Horario = dto.Horario,
                ID_Periodo = dto.ID_Periodo
            };

            var result = await _repo.UpdateAsync(actualizado) > 0;

            if (result)
            {
                var descripcion = $"Actualizó grupo: Antes={JsonSerializer.Serialize(anterior)}, Ahora={JsonSerializer.Serialize(actualizado)}";
                await RegistrarBitacoraAsync(usuario, descripcion);
            }

            return result;
        }

        // 🔹 Eliminar grupo
        public async Task<bool> EliminarAsync(int id, string usuario)
        {
            var grupo = await _repo.GetByIdAsync(id);
            if (grupo is null)
                throw new ArgumentException("No existe un grupo con el ID indicado.");

            var result = await _repo.DeleteAsync(id) > 0;

            if (result)
                await RegistrarBitacoraAsync(usuario, $"Eliminó grupo: {JsonSerializer.Serialize(grupo)}");

            return result;
        }

        // 🔹 Validación interna
        private static void ValidarGrupo(GrupoCreateDto dto)
        {
            if (dto.Numero_Grupo <= 0)
                throw new ArgumentException("El número de grupo debe ser mayor a 0.");

            if (dto.ID_Curso <= 0)
                throw new ArgumentException("Debe indicar un curso válido.");

            if (dto.ID_Profesor <= 0)
                throw new ArgumentException("Debe indicar un profesor válido.");

            if (dto.ID_Periodo <= 0)
                throw new ArgumentException("Debe indicar un periodo válido.");

            if (string.IsNullOrWhiteSpace(dto.Horario))
                throw new ArgumentException("El horario es obligatorio.");
        }

        // 🔹 Método auxiliar para registrar bitácoras (GEN1)
        private async Task RegistrarBitacoraAsync(string idUsuario, string accion)
        {
            try
            {
                await _bitacoraClient.PostAsJsonAsync("/bitacora", new
                {
                    ID_Usuario = idUsuario,
                    Accion = accion
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al registrar bitácora: {ex.Message}");
            }
        }
    }
}
