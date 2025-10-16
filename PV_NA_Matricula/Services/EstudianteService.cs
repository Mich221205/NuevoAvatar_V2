using PV_NA_Matricula.Entities;
using PV_NA_Matricula.Repository;
using System.Text.RegularExpressions;

namespace PV_NA_Matricula.Services
{
	public class EstudianteService : IEstudianteService
	{
		private readonly IEstudianteRepository _repo;

		public EstudianteService(IEstudianteRepository repo)
		{
			_repo = repo;
		}

		public async Task<IEnumerable<Estudiante>> GetAllAsync() => await _repo.GetAllAsync();

		public async Task<Estudiante?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

		public async Task<int> CreateAsync(Estudiante e)
		{
			ValidarEstudiante(e);
			return await _repo.InsertAsync(e);
		}

		public async Task<int> UpdateAsync(Estudiante e)
		{
			ValidarEstudiante(e);
			return await _repo.UpdateAsync(e);
		} 

		public async Task<int> DeleteAsync(int id) => await _repo.DeleteAsync(id);

		// Método privado con todas las validaciones de la HU MAT3
		private void ValidarEstudiante(Estudiante e)
		{
			// Validar campos requeridos
			if (string.IsNullOrWhiteSpace(e.Identificacion) ||
				string.IsNullOrWhiteSpace(e.Tipo_Identificacion) ||
				string.IsNullOrWhiteSpace(e.Email) ||
				string.IsNullOrWhiteSpace(e.Nombre) ||
				string.IsNullOrWhiteSpace(e.Direccion) ||
				string.IsNullOrWhiteSpace(e.Telefono))
			{
				throw new Exception("Todos los campos son requeridos.");
			}

			// Validar formato del nombre
			var regexNombre = new Regex(@"^[A-Za-zÁÉÍÓÚáéíóúñÑ ]+$");
			if (!regexNombre.IsMatch(e.Nombre))
				throw new Exception("El nombre solo puede contener letras y espacios.");

			// Validar formato del correo
			if (!Regex.IsMatch(e.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
				throw new Exception("El formato del correo electrónico no es válido.");

			// Validar dominio del correo
			if (!e.Email.EndsWith("@cuc.cr", StringComparison.OrdinalIgnoreCase))
				throw new Exception("El correo debe pertenecer al dominio @cuc.cr.");

			// Validar mayoría de edad
			int edad = DateTime.Now.Year - e.Fecha_Nacimiento.Year;
			if (e.Fecha_Nacimiento > DateTime.Now.AddYears(-edad))
				edad--;
			if (edad < 18)
				throw new Exception("El estudiante debe ser mayor de edad.");
		}
	}
}
