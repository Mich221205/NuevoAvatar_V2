using PV_NA_OfertaAcademica.Dtos;
using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Repository;
using System.Net.Http.Json;
using System.Text.Json;

namespace PV_NA_OfertaAcademica.Services
{
    public class PeriodoService
    {
        private readonly PeriodoRepository _repo;
        private readonly HttpClient _bitacoraClient;

        public PeriodoService(PeriodoRepository repo, IHttpClientFactory httpClientFactory)
        {
            _repo = repo;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<Periodo>> ObtenerTodosAsync(string usuario)
        {
            var lista = await _repo.GetAllAsync();
            await RegistrarBitacoraAsync(usuario, "Consultó listado de periodos");
            return lista;
        }

        public async Task<Periodo?> ObtenerPorIdAsync(int id, string usuario)
        {
            var periodo = await _repo.GetByIdAsync(id);
            await RegistrarBitacoraAsync(usuario, $"Consultó periodo con ID {id}");
            return periodo;
        }

        public async Task CrearAsync(PeriodoCreateDto dto, string usuario)
        {
            if (dto.Fecha_Fin <= dto.Fecha_Inicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");

            var periodo = new Periodo
            {
                Anno = dto.Anio,
                Numero = dto.Numero_Periodo,
                Fecha_Inicio = dto.Fecha_Inicio,
                Fecha_Fin = dto.Fecha_Fin
            };

            await _repo.CreateAsync(periodo);
            await RegistrarBitacoraAsync(usuario, $"Creó periodo: {JsonSerializer.Serialize(periodo)}");
        }

        public async Task ModificarAsync(PeriodoUpdateDto dto, string usuario)
        {
            if (dto.Fecha_Fin <= dto.Fecha_Inicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio.");

            var anterior = await _repo.GetByIdAsync(dto.ID_Periodo);

            var periodo = new Periodo
            {
                ID_Periodo = dto.ID_Periodo,
                Anno = dto.Anio,
                Numero = dto.Numero_Periodo,
                Fecha_Inicio = dto.Fecha_Inicio,
                Fecha_Fin = dto.Fecha_Fin
            };

            await _repo.UpdateAsync(periodo);

            var descripcion = $"Actualizó periodo: Antes={JsonSerializer.Serialize(anterior)}, Ahora={JsonSerializer.Serialize(periodo)}";
            await RegistrarBitacoraAsync(usuario, descripcion);
        }

        public async Task EliminarAsync(int id, string usuario)
        {
            var periodo = await _repo.GetByIdAsync(id);
            await _repo.DeleteAsync(id);

            await RegistrarBitacoraAsync(usuario, $"Eliminó periodo: {JsonSerializer.Serialize(periodo)}");
        }

        // -------------------------------
        // 🔹 Método auxiliar de bitácora
        // -------------------------------
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
