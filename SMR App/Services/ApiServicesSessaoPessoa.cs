using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMRDominio;
using SMRDominio.ClassePessoa;

namespace SMR_App.Services
{
    public static class ApiServicesSessaoPessoa
    {
        public static Pessoa? PessoaLogada { get; private set; }

        public static event Action? OnSessaoChanged;

        public static void IniciarSessao(Pessoa pessoa)
        {
            PessoaLogada = pessoa;
            OnSessaoChanged?.Invoke();
        }

        public static void EncerrarSessao() // Método de Logout unificado
        {
            PessoaLogada = null;
            OnSessaoChanged?.Invoke(); // Dispara o evento de mudança
        }

        //public static void Logout()
        //{
        //    PessoaLogada = null;
        //}
    }
}
