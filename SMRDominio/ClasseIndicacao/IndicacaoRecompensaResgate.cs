using SMRDominio.ClasseRecompensa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseIndicacao
{
    public class IndicacaoRecompensaResgate
    {
        public int IDRecompensa { get; set; }
        public Recompensa_Rank IDRank { get; set; }
        public int IDEmpresa { get; set; }
        public string RazaoSocial { get; set; }
        public int PontosNecessarios { get; set; }

        // NOVO: Propriedade para controlar a tela
        public bool PodeResgatar { get; set; }
    }
    public class ConsultaFinalResgate
    {
        public List<IndicacaoRecompensaResgate>? ListaRecompensas { get; set; }
        public List<PromotorPontosResgate>? PromotorPontos { get; set; }
    }
    public class PromotorPontosResgate
    {
        public int IDPromotor { get; set; }
        public int IDEmpresa { get; set; }
        public int PontosAcumulados { get; set; }
        public Recompensa_Rank? IDPromotorRank { get; set; }
    }    
}
