using SMRDominio.ClasseRecompensa;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseIndicacao
{
    public class IndicacaoHistoricoDto 
    {
        public int IDIndicacao { get; set; }
        public int IdPromotor { get; set; }
        public string? NomeIndicado { get; set; }
        public DateTime DataIndicacao { get; set; }
        public DateTime? DataValidacao { get; set; }
        public int IdBonificacao { get; set; }
        public string? DescricaoBonificacao { get; set; }
        public int IdEmpresa { get; set; }
        public string? RazaoSocial { get; set; }
        public IndicacaoStatus IDSituacaoIndicacao { get; set; }
    }
    public class PromotorPontosResponse
    {
        public int IDEmpresa { get; set; }
        public string RazaoSocial { get; set; }
        public int PontosAcumulados { get; set; }
        public Recompensa_Rank? IDPromotorRank { get; set; }
    }

    public class RecompensaResponse
    {
        public int IDRecompensa { get; set; }
        public int IDEmpresa { get; set; }
        public Recompensa_Rank IDRank { get; set; }
        public int PontosNecessarios { get; set; }
    }

    public class ConsultaFinalHistorico
    {
        public List<RecompensaResponse> RecompensasEmpresas { get; set; }
        public List<IndicacaoHistoricoDto> Indicacoes { get; set; }
        public List<PromotorPontosResponse> PontosPromotor { get; set; }
    }
}
