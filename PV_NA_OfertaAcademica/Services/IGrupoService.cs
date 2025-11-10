using PV_NA_OfertaAcademica.Dtos;
using PV_NA_OfertaAcademica.Entities;

namespace PV_NA_OfertaAcademica.Services
{ 
    /*
    public interface IGrupoService
    {
        Task<IEnumerable<Grupo>> GetAll(string usuario);
        Task<Grupo?> GetById(int id, string usuario);
        Task<bool> Create(GrupoCreateDto dto, string usuario);
        Task<bool> Update(int id, GrupoUpdateDto dto, string usuario);
        Task<bool> Delete(int id, string usuario);
    }*/

    public interface IGrupoService
    {
        Task<IEnumerable<Grupo>> ObtenerTodosAsync(string usuario);
        Task<Grupo?> ObtenerPorIdAsync(int id, string usuario);
        Task<bool> CrearAsync(GrupoCreateDto dto, string usuario);
        Task<bool> ModificarAsync(GrupoUpdateDto dto, string usuario);
        Task<bool> EliminarAsync(int id, string usuario);
    }
}
