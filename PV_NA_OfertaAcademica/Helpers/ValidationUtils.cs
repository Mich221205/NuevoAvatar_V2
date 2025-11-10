using System.Text.RegularExpressions;

namespace PV_NA_OfertaAcademica.Helpers
{
    // ValidationUtils contiene métodos estáticos para validar datos de cursos.
    public static class ValidationUtils
    {
        // Expresión regular para validar que el nombre solo contenga letras y espacios, incluyendo caracteres acentuados y ñ.
        static readonly Regex _onlyLettersSpaces = new(@"^[A-Za-zÁÉÍÓÚáéíóúÜüÑñ ]+$", RegexOptions.Compiled);

        public static bool NombreValido(string nombre)
            => !string.IsNullOrWhiteSpace(nombre) && nombre.Length <= 100 && _onlyLettersSpaces.IsMatch(nombre);

        public static bool NivelValido(int nivel) => nivel >= 1 && nivel <= 12;
    }
    
}
