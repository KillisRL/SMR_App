using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SMRDominio.ClassePessoa
{
    [Table("promotor")]
    public class Promotor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [Column("id_pessoa")]
        public int id_pessoa { get; set; }
        [Required]
        public string? cpf { get; set; }
<<<<<<< HEAD
        public string? nome { get; set; }
=======
>>>>>>> dfa26fb (criação da service e api de recompensas)
        public string? celular { get; set; }
        public int pontos_acumulados { get; set; }
    }
}
