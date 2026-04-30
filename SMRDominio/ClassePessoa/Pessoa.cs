using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClassePessoa
{
    [Table ("Pessoa")]
    public class Pessoa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_pessoa { get; set; }
        [Required]
        [Column("id_pessoatipo")]
        public PessoaTipo id_pessoa_tipo { get; set; }
        public string? nome { get; set; }
        public string? celular { get; set; }
        public string? email { get; set; }
        public string? senha_hash { get; set; }
        public string? login { get; set; }
        public bool? ativo { get; set; }
        public DateTime data_cadastro { get; set; }
    }
}
