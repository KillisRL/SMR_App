using System.ComponentModel.DataAnnotations.Schema;

namespace SMRDominio.ClassePessoa
{
    [Table("Pessoa")]
    public class PessoaLogin
    {
        public string documento { get; set; }
        public string senha_hash { get; set; }
    }
}
