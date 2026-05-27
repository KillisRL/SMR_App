using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMRDominio.ClassePessoa
{
    [Table ("empresa")]
    public class Empresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [Column("id_pessoa")]
        public int id_pessoa { get; set; }
        [Required]
        public string? razao_social { get; set; }
        public string? nome_fantasia { get; set; }
        [Required]
        public string? cnpj {  get; set; }
        public string? cor_padrao { get; set; }
        public string? telefone1 { get; set; }
        public string? telefone2 { get; set; }
        

    }
}
