<<<<<<< HEAD
<<<<<<< HEAD
﻿using SMR_App.Services;
=======
﻿using CommunityToolkit.Mvvm.ComponentModel;
using SMR_App.Services;
>>>>>>> dfa26fb (criação da service e api de recompensas)
=======
﻿using CommunityToolkit.Mvvm.ComponentModel;
using SMR_App.Services;
>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
using SMRDominio.ClassePessoa;

namespace SMR_App.ViewModels
{
    public class PrincipalViewModel : BaseViewModel
    {
<<<<<<< HEAD
<<<<<<< HEAD
=======

>>>>>>> dfa26fb (criação da service e api de recompensas)
=======

>>>>>>> 2ad720e3daa17187a5b64c6d7f8bffd91c473d34
        // VARIAVEIS
        private bool _visualizarConfiguracoesEmpresa;

        // PROPRIEDADES
        public bool VisualizarConfiguracoesEmpresa
        {
            get => _visualizarConfiguracoesEmpresa;
            set
            {
                _visualizarConfiguracoesEmpresa = value;
                OnPropertyChanged();
            }
        }

        

        public PrincipalViewModel()
        {
            ValidarPermissoesDeMenu();
        }

        private void ValidarPermissoesDeMenu()
        {
            // Obter pessoa logada
            var pessoa = ApiServicesSessaoPessoa.PessoaLogada;

            // Se for pessoa jurídica aparece o menu de configuração de empresa
            if (pessoa != null)
            {
                VisualizarConfiguracoesEmpresa = (pessoa.id_pessoa_tipo == PessoaTipo.Empresa);

                NomeUsuario = pessoa.nome;
            }
            else
            {
                VisualizarConfiguracoesEmpresa = false;
            }
        }
    }
}
