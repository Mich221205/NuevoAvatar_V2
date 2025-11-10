using PV_NA_OfertaAcademica.Entities;
using PV_NA_OfertaAcademica.Repository;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Json;

namespace PV_NA_OfertaAcademica.Services
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio para instituciones.
    /// Incluye validaciones y registro automático en bitácora (GEN1).
    /// </summary>
    public class InstitucionService : IInstitucionService
    {
        private readonly IInstitucionRepository _repository;
        private readonly HttpClient _bitacoraClient;

        public InstitucionService(IInstitucionRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _bitacoraClient = httpClientFactory.CreateClient("BitacoraClient");
        }

        //  Listar todas las instituciones
        public async Task<IEnumerable<Institucion>> GetAll(string usuario)
        {
            var lista = await _repository.GetAllAsync();
            await RegistrarBitacoraAsync(usuario, "Consultó listado de instituciones");
            return lista;
        }

        //  Obtener una institución por ID
        public async Task<Institucion?> GetById(int id, string usuario)
        {
            var inst = await _repository.GetByIdAsync(id);
            await RegistrarBitacoraAsync(usuario, $"Consultó institución con ID {id}");
            return inst;
        }

        //  Crear institución (con validación y bitácora)
        public async Task<bool> Create(Institucion institucion, string usuario)
        {
            ValidarInstitucion(institucion);

            var id = await _repository.CreateAsync(institucion);
            var creada = id > 0;

            if (creada)
                await RegistrarBitacoraAsync(usuario, $"Creó institución: {JsonSerializer.Serialize(institucion)}");

            return creada;
        }

        //  Actualizar institución (con validación y bitácora)
        public async Task<bool> Update(Institucion institucion, string usuario)
        {
            ValidarInstitucion(institucion);

            var anterior = await _repository.GetByIdAsync(institucion.ID_Institucion);
            var actualizada = await _repository.UpdateAsync(institucion);

            if (actualizada && anterior != null)
            {
                var descripcion = $"Actualizó institución: Antes={JsonSerializer.Serialize(anterior)}, Ahora={JsonSerializer.Serialize(institucion)}";
                await RegistrarBitacoraAsync(usuario, descripcion);
            }

            return actualizada;
        }

        // Eliminar institución (con bitácora)
        public async Task<bool> Delete(int id, string usuario)
        {
            var inst = await _repository.GetByIdAsync(id);
            var eliminada = await _repository.DeleteAsync(id);

            if (eliminada && inst != null)
                await RegistrarBitacoraAsync(usuario, $"Eliminó institución: {JsonSerializer.Serialize(inst)}");

            return eliminada;
        }

        //  Validar nombre no vacío y solo letras/espacios
        private void ValidarInstitucion(Institucion inst)
        {
            if (string.IsNullOrWhiteSpace(inst.Nombre))
                throw new ArgumentException("El nombre de la institución es obligatorio.");

            if (!Regex.IsMatch(inst.Nombre, @"^[A-Za-z\s]+$"))
                throw new ArgumentException("El nombre solo puede contener letras y espacios.");
        }

        //  Registrar acción en la bitácora (GEN1)
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
                Console.WriteLine($"⚠️ Error al registrar bitácora: {ex.Message}");
            }
        }
    }
}
