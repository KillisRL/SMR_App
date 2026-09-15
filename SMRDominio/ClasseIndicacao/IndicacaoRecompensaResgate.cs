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
        public double AlturaPreenchimento { get; set; }
        public bool PodeResgatar { get; set; }
        public bool JaResgatada { get; set; }
        public string TextoBotaoResgate => JaResgatada ? "RESGATADA" : "RESGATAR!";
    }
    public class ConsultaFinalResgate
    {
        public List<IndicacaoRecompensaResgate>? ListaRecompensas { get; set; }
        public List<PromotorPontosResgate>? PromotorPontos { get; set; }
        public List<RecompensaResgatadas> Resgatadas { get; set; }
    }
    public class PromotorPontosResgate
    {
        public int IDPromotor { get; set; }
        public int IDEmpresa { get; set; }
        public int PontosAcumulados { get; set; }
        public Recompensa_Rank? IDPromotorRank { get; set; }
    }
    
    public class RecompensaResgatadas
    {
        public int IDPromotor { get; set; }
        public int IDRecompensa { get; set; }
        public int IDEmpresa { get; set; }
    }
}
