using SMRDominio.ClasseBase;
using SMRDominio.ClasseRecompensa;
using SMRDominio.DTOs;
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
    public partial class ApiServiceRecompensa
    {
        private readonly HttpClient _httpClient;

        public ApiServiceRecompensa(HttpClient httpClient)
        {
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

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseURL)
            };
        }

        public async Task<(bool Sucesso, string Mensagem)> ExcluirRecompensa(string token, int codigoRecompensa)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resultado = await _httpClient.DeleteAsync($"recompensas/excluir{codigoRecompensa}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (resultado.IsSuccessStatusCode)
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagemSucesso = retorno?.Mensagem ?? "Recompensa excluída com sucesso.";
                    return (true, mensagemSucesso);
                }
                else
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagemErro = retorno?.Mensagem ?? "Não foi possível excluir a recompensa";
                    return (false, mensagemErro);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.");
            }
        }

        public async Task<(bool Sucesso, string Mensagem)> AlterarRecompensa(string token, Recompensa recompensaAlterada)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resultado = await _httpClient.PutAsJsonAsync("recompensas/alterar", recompensaAlterada);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (resultado.IsSuccessStatusCode)
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagemSucesso = retorno?.Mensagem ?? "Recompensa alterada com sucesso.";
                    return (true, mensagemSucesso);
                }
                else
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagemErro = retorno?.Mensagem ?? "Falha para alterada recompensa.";

                    return (false, mensagemErro);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.");
            }
        }

        public async Task<(bool Sucesso, string Mensagem)> CadastrarRecompensa(string token, Recompensa recompensaNova)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var resultado = await _httpClient.PostAsJsonAsync("recompensas/cadastrar", recompensaNova);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (resultado.IsSuccessStatusCode)
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagemSucesso = retorno?.Mensagem ?? "Recompensa cadastrada com sucesso.";
                    return (true, mensagemSucesso);
                }
                else
                {
                    var retorno = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string mensagemErro = retorno?.Mensagem ?? "Falha ao cadastrar Recompensa.";
                    return (false, mensagemErro);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao cadastrar: {ex.Message}");
                return (false, "Falha de comunicação com o servidor.");
            }
        }

        public async Task<(bool Sucesso, List<Recompensa> Dados, string Mensagem)> ConsultarRecompensas(string token, string? descricao, string? titulo, bool? ativo)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var query = new List<string>();

                // 1. Correção dos parâmetros da Query String (adicionado '=' e EscapeDataString)
                if (!string.IsNullOrWhiteSpace(descricao))
                {
                    query.Add($"descricao={Uri.EscapeDataString(descricao)}");
                }

                if (!string.IsNullOrWhiteSpace(titulo))
                {
                    query.Add($"titulo={Uri.EscapeDataString(titulo)}");
                }

                if (ativo.HasValue)
                {
                    // Converte bool para string em minúsculo ("true" ou "false")
                    query.Add($"ativo={ativo.Value.ToString().ToLower()}");
                }

                // Monta a URL (ex: recompensa/consultar?descricao=desconto&ativo=true)
                var urlCompleta = query.Any()
                    ? $"recompensas/consultar?{string.Join("&", query)}"
                    : "recompensas/consultar";

                var resultado = await _httpClient.GetAsync(urlCompleta);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                if (resultado.IsSuccessStatusCode)
                {
                    // Retorna a lista tratada (se vier nula, retorna lista vazia para evitar NullReference)
                    var dados = await resultado.Content.ReadFromJsonAsync<List<Recompensa>>(options);
                    return (true, dados ?? new List<Recompensa>(), string.Empty);
                }
                else
                {
                    var apiResposta = await resultado.Content.ReadFromJsonAsync<ApiRetornoMensagem>(options);
                    string textoMensagem = apiResposta?.Mensagem ?? "Falha ao consultar Recompensa.";

                    return (false, null, textoMensagem);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceção ao consultar detalhes: {ex.Message}");
                return (false, null, "Falha interna ao consultar recompensa.");
            }
        }
    }
}
