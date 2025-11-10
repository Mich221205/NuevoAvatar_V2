namespace PV_NA_Pagos.Entities
{
    public class Pago
    {
        public int ID_Pago { get; set; }
        public int ID_Factura { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
