using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMRDominio.ClasseIndicacao
{
    [Table("indicacao")]
    public class Indicacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Id_Promotor_Indicou { get; set; }
        [ForeignKey("id_bonificacao")]
        public int Id_Bonificacao { get; set; }

        public string? Nome_Indicado { get; set; }

        public string? Telefone_Indicado { get; set; }

        public string? Codigo_Validacao { get; set; }

        public IndicacaoStatus? Status_Indicacao { get; set; }

        public DateTime Data_Indicacao { get; set; }

        public DateTime? Data_Validacao { get; set; }

        public string? CPF { get; set; }
    }
}