using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseBonificacao
{
    [Table("bonificacao")]
    public class Bonificacao
    {
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int Id_Empresa { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public TipoBonificacao Tipo { get; set; }
        public bool Mgm { get; set; }
        public bool Ativo { get; set; }
    }
}
