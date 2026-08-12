using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
