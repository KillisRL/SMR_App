using SMR_App.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMR_App.ViewModels
{
    public partial class GrRecompensasViewModel : BaseViewModel
    {
        private readonly ApiServiceRecompensa _apiServiceRecompensa;

        public GrRecompensasViewModel(ApiServiceRecompensa apiServiceRecompensa) 
        {
            _apiServiceRecompensa = apiServiceRecompensa;
        }



    }
}
