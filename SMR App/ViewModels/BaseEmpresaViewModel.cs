using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SMRDominio.ClassePessoa;
using System.Collections.Generic;
using System.Linq;

namespace SMR_App.ViewModels
{
    public abstract partial class BaseEmpresaViewModel : BaseViewModel
    {
        // Lista com os nomes ou rotas dos relatórios disponíveis no sistema
        protected List<string> _rotasRelatorios = new()
        {
            "RelatoriosCoBonifView", // Exemplo 1
            "OutroRelatorioView"     // Exemplo 2 (futuros relatórios)
        };

        protected int _indexRelatorioAtual = 0;

        [RelayCommand]
        public virtual async Task ProximaEmpresa() // Mantém o nome ou muda para ProximoRelatorio se preferir
        {
            if (_rotasRelatorios.Count <= 1) return;

            _indexRelatorioAtual++;
            if (_indexRelatorioAtual >= _rotasRelatorios.Count)
                _indexRelatorioAtual = 0;

            await NavegarParaRelatorioAtual();
        }

        [RelayCommand]
        public virtual async Task EmpresaAnterior() // Mantém o nome ou muda para RelatorioAnterior
        {
            if (_rotasRelatorios.Count <= 1) return;

            _indexRelatorioAtual--;
            if (_indexRelatorioAtual < 0)
                _indexRelatorioAtual = _rotasRelatorios.Count - 1;

            await NavegarParaRelatorioAtual();
        }

        private async Task NavegarParaRelatorioAtual()
        {
            string rotaDestino = _rotasRelatorios[_indexRelatorioAtual];
            await Shell.Current.GoToAsync(nameof(rotaDestino));
        }
    }
}