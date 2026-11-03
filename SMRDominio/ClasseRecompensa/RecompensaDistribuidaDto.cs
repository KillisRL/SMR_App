using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMRDominio.ClasseRecompensa
{
    public class RecompensaDistribuidaDto
    {
        public int IdResgate { get; set; }
        public string TituloRecompensa { get; set; }
        public string NomePromotor { get; set; }
        public DateTime DataResgate { get; set; }
    }
}
