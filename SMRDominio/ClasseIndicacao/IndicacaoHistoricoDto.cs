using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseIndicacao
{
    public class IndicacaoHistoricoDto
    {
        public int IdPromotor { get; set; }
        public string? NomeIndicado { get; set; }
        public DateTime DataIndicacao { get; set; }
        public DateTime? DataValidacao { get; set; }
        public int IdBonificacao { get; set; }
        public string? DescricaoBonificacao { get; set; }
        public int IdEmpresa { get; set; }
        public string? RazaoSocial { get; set; }
    }
}
