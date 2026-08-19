using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseIndicacao
{
    public class IndicacaoDetalhesDto
    {
        public int IDIndicacao { get; set; }
        public string NomeIndicado { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string TelefoneIndicado { get; set; } = string.Empty;
        public string StatusIndicacao { get; set; } = string.Empty;
        public DateTime DataIndicacao { get; set; }
        public DateTime? DataValidacao { get; set; }

        // Dados da Bonificação
        public int IDBonificacao { get; set; }
        public string NomeBonificacao { get; set; } = string.Empty;
        public string? DescricaoBonificacao { get; set; }
        public decimal ValorBonificacao { get; set; }

        // Dados da Empresa
        public int IDEmpresa { get; set; }
        public string RazaoSocial { get; set; } = string.Empty;

        // Dados do Promotor
        public int IDPromotor { get; set; }
        public string NomePromotor { get; set; } = string.Empty;
    }
}
