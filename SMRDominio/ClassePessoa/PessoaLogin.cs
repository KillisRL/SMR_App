using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClassePessoa
{
    [Table("Pessoa")]
    public class PessoaLogin
    {
        public string login { get; set; }
        public string senha_hash { get; set; }
    }
}
