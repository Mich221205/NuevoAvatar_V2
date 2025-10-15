namespace PV_NA_UsuariosRoles.Entities
{
    public class Sesion
    {
        public int ID_Sesion { get; set; }
        public int ID_Usuario { get; set; }
        public string Token_JWT { get; set; }
        public string Refresh_Token { get; set; }
        public DateTime Expira { get; set; }
    }
}
