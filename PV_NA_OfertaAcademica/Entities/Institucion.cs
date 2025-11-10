using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PV_NA_OfertaAcademica.Entities
{
    public class Institucion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID_Institucion { get; set; }
        public string Nombre { get; set; }
    }
}
