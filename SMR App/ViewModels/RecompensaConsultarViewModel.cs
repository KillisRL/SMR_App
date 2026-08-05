using SMR_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    public class RecompensaConsultarViewModel : BaseViewModel
    {
        private readonly ApiServiceRecompensa _apiServiceRecompensa;

        public RecompensaConsultarViewModel(ApiServiceRecompensa apiServiceRecompensa) 
        { 
            _apiServiceRecompensa = apiServiceRecompensa;
        }

    }
}
