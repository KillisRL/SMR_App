using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseBonificacao
{
    public class Bonificacao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool IsMgm { get; set; }
        public bool IsAtivo { get; set; }
    }
}
