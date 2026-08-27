using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClassePessoa
{
    [Table("promotor_pontos")]
    public class PromotorPontos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_promotor_pontos { get; set; }
        [ForeignKey("id_promotor")]
        public int id_promotor { get; set; }

        [ForeignKey("id_empresa")]
        public int id_empresa { get; set; }
        public int pontos_acumulados { get; set; }
        public DateTime data_atualizacao { get; set; }
    }
}
