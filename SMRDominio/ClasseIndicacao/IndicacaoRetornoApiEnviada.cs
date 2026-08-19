using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseIndicacao
{
    public class IndicacaoRetornoApiEnviada
    {
        public string? Mensagem { get; set; }
        public string? CodigoValidacao { get; set; }
        public string? LinkValidacao { get; set; }
        public int? IDIndicacao { get; set; }
    }
}
