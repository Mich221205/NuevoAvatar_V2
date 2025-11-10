namespace PV_NA_Pagos.Entities
{
    public class Factura
    {
        public int ID_Factura { get; set; }
        public int ID_Estudiante { get; set; }
        public decimal Monto { get; set; }
        public decimal Impuesto { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
