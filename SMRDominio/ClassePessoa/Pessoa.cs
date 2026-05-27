using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMRDominio.ClassePessoa
{
    [Table ("pessoa")]
    public class Pessoa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_pessoa { get; set; }
        [Required]
        [Column("id_pessoa_tipo")]
        public PessoaTipo id_pessoa_tipo { get; set; }
        public string? nome { get; set; }
        public string? email { get; set; }
        public string? senha_hash { get; set; }
        public bool? ativo { get; set; }
        public DateTime data_cadastro { get; set; }

    }
}
