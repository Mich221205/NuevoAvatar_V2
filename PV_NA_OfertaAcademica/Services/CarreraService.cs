using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Repository;
using System;
using System.Net.Http.Json;
using System.Text.Json;

namespace PV_NA_OfertaAcademica.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio para carreras.
    /// Incluye validaciones y registro automático de bitácoras sin depender de un servicio externo.
    /// </summary>
    public class CarreraService : ICarreraService
    {
        private readonly ICarreraRepository _repository;
        private readonly HttpClient _bitacoraClient;

        public CarreraService(ICarreraRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        public async Task<IEnumerable<Carrera>> GetAll(string usuario)
        {
            var lista = await _repository.GetAllAsync();
            await RegistrarBitacoraAsync(usuario, "Consultó listado de carreras");
            return lista;
        }

        public async Task<Carrera?> GetById(int id, string usuario)
        {
            var carrera = await _repository.GetByIdAsync(id);
            await RegistrarBitacoraAsync(usuario, $"Consultó carrera con ID {id}");
            return carrera;
        }

        public async Task<IEnumerable<Carrera>> GetByInstitucion(int idInstitucion, string usuario)
        {
            var carreras = await _repository.GetByInstitucionAsync(idInstitucion);
            await RegistrarBitacoraAsync(usuario, $"Consultó carreras por institución ID {idInstitucion}");
            return carreras;
        }

        public async Task<bool> Create(Carrera carrera, string usuario)
        {
            if (string.IsNullOrWhiteSpace(carrera.Nombre) ||
                !carrera.Nombre.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                throw new ArgumentException("Nombre inválido.");

            var ok = await _repository.CreateAsync(carrera) > 0;

            if (ok)
                await RegistrarBitacoraAsync(usuario, $"Creó carrera: {JsonSerializer.Serialize(carrera)}");

            return ok;
        }

        public async Task<bool> Update(Carrera carrera, string usuario)
        {
            var anterior = await _repository.GetByIdAsync(carrera.ID_Carrera);
            var ok = await _repository.UpdateAsync(carrera) > 0;

            if (ok)
            {
                var descripcion = $"Actualizó carrera: Antes={JsonSerializer.Serialize(anterior)}, Ahora={JsonSerializer.Serialize(carrera)}";
                await RegistrarBitacoraAsync(usuario, descripcion);
            }

            return ok;
        }

        public async Task<bool> Delete(int id, string usuario)
        {
            var carrera = await _repository.GetByIdAsync(id);
            var ok = await _repository.DeleteAsync(id) > 0;

            if (ok)
                await RegistrarBitacoraAsync(usuario, $"Eliminó carrera: {JsonSerializer.Serialize(carrera)}");

            return ok;
        }

        // Método auxiliar para enviar registros a la bitácora (GEN1)
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
                Console.WriteLine($" Error al registrar bitácora: {ex.Message}");
            }
        }
    }
}
