using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseRecompensa
{
    [Table("recompensa_resgate")]
    public class RecompensaResgate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_recompensa_resgate { get; set; }
        [ForeignKey("id_promotor")]
        public int? id_promotor { get; set; }
        [ForeignKey("id_empresa")]
        public int id_empresa { get; set; }
        [ForeignKey("id_recompensa")]
        public int id_recompensa { get; set; }
        public DateTime data_resgate { get; set; }
    }
}
