namespace PV_NA_UsuariosRoles.Entities
{
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public string Email { get; set; } = null!;
        public string Tipo_Identificacion { get; set; } = null!;
        public string Identificacion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public int ID_Rol { get; set; }
        public string? RolNombre { get; set; } // campo auxiliar para joins
    }
}


