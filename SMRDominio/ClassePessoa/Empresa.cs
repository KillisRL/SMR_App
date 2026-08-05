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
<<<<<<< HEAD
<<<<<<< HEAD
        public string? nome_fantasia { get; set; }
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        [Required]
        public string? cnpj {  get; set; }
        public string? cor_padrao { get; set; }
        public string? telefone1 { get; set; }
        public string? telefone2 { get; set; }
        

    }
}
