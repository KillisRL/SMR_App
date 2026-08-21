using SMRDominio.ClasseBase;
using SMRDominio.ClasseIndicacao;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SMR_App.Services
{
    public class ApiServiceIndicacao
    {

        private readonly HttpClient _httpClient;

        public ApiServiceIndicacao(HttpClient httpClient)
        {
<<<<<<< Updated upstream
            // Define os endereços
            string urlLocal = "https://localhost:7190/";
            string urlProducao = "https://Api-smr-backend-env.eba-fihsn5vm.sa-east-1.elasticbeanstalk.com/";

            // Lógica inteligente: 
            // Se for Android, usa a AWS. Se for Windows (ou outro), usa Localhost.
            string baseURL = (DeviceInfo.Platform == DevicePlatform.Android)
                ? urlProducao
                : urlLocal;

            var handler = new HttpClientHandler
            {
                UseProxy = false
            };

#if DEBUG
            // Ignora o certificado SSL apenas em modo Debug para facilitar a vida
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
=======
            var handler = new HttpClientHandler();
            handler.UseProxy = false;
>>>>>>> Stashed changes

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(ConfiguracoesApp.UrlApi)
            };
        }
        public async Task<(bool Sucesso, IndicacaoRetornoApiEnviada Dados)> ConsultarIndicacaoValidacao(string token, string codigoValidacao)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resultado = await _httpClient.GetAsync($"indicacao/consultar-validacao/{codigoValidacao}");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if(resultado.IsSuccessStatusCode)
                {
                    var dados  = await resultado.Content.ReadFromJsonAsync<IndicacaoRetornoApiEnviada>(options);

                    return (true,  dados);
                }
                else
                {
                    var dados = await resultado.Content.ReadFromJsonAsync<IndicacaoRetornoApiEnviada>(options);
                    return (false, dados );
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao consultar indicação: {ex.Message}");
                return (false, null);
            }
        }
        public async Task<(bool Sucesso, string Mensagem)> ConfirmarValidacaoPorCodigo(string token, string codigo)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var payload = new { CodigoValidacao = codigo };
                var resultado = await _httpClient.PostAsJsonAsync("indicacao/validar/confirmar", payload);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                if(resultado.IsSuccessStatusCode)
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    return (true, retorno?.Mensagem ?? "Indicação validada com sucesso!");
                }
                else
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    return (false, retorno?.Mensagem ?? "Não foi possível validar esta indicação.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao alterar status: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.");
            }
        }
        public async Task<(bool Sucesso, string Mensagem, IndicacaoRetornoApiEnviada? Dados)> IndicacaoAlterarStatus(string token, int codigoIndicacao, IndicacaoStatus indicacaoStatus)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Envia o enum diretamente como JSON (sem StringContent manual)
                var resultado = await _httpClient.PutAsJsonAsync($"indicacao/alterar-status/{codigoIndicacao}", indicacaoStatus);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dados = await resultado.Content.ReadFromJsonAsync<IndicacaoRetornoApiEnviada>(options);

                if (resultado.IsSuccessStatusCode && dados != null)
                {
                    return (true, dados.Mensagem ?? "Status alterado com sucesso!", dados);
                }
                else
                {
                    return (false, dados?.Mensagem ?? "Não foi possível alterar a situação.", null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao alterar status: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.", null);
            }
        }

        public async Task<(bool Sucesso, string Mensagem, IndicacaoDetalhesDto Dados)> ConsultarIndicacaoDetalhes(string token, int codigoIndicacao)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resultado = await _httpClient.GetAsync($"indicacao/consultar-detalhes/{codigoIndicacao}");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if(resultado.IsSuccessStatusCode)
                {
                    var dados = await resultado.Content.ReadFromJsonAsync<IndicacaoDetalhesDto>(options);
                    return (true, string.Empty, dados);
                }
                else
                {
                    var mensagemErro = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    return (false, mensagemErro.Mensagem, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.", null);
            }
        }
        public async Task<(bool Sucesso, string Mensagem, int codigoIndicacao)> CadastrarIndicacao(string token, Indicacao indicacaoCadastrar)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resultado = await _httpClient.PostAsJsonAsync("indicacao/cadastrar", indicacaoCadastrar);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (resultado.IsSuccessStatusCode)
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<IndicacaoRetornoApiCadastro>(options);
                    string mensagem = retorno.Mensagem;
                    int codigoIndicacao = retorno.CodigoIndicacao;
                    return (true, mensagem, codigoIndicacao);

                }
                else
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagem = retorno.Mensagem;

                    return (false, mensagem, 0);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.", 0);
            }
        }

    }
}
