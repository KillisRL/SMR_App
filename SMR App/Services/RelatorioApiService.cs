using SMR_App.Models;
using SMRDominio.ClasseBonificacao;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SMR_App.Services
{
    public class RelatorioApiService
    {
        private readonly HttpClient _httpClient;

        public RelatorioApiService()
        {
            var handler = new HttpClientHandler { UseProxy = false };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(ConfiguracoesApp.UrlApi)
            };
        }

        public async Task<byte[]?> BaixarRelatorioExcelAsync(DateTime inicio, DateTime fim, string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string url = $"relatorios/exportar-excel?inicio={inicio:yyyy-MM-dd}&fim={fim:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao baixar Excel: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]?> BaixarRelatorioPdfAsync(DateTime inicio, DateTime fim, string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string url = $"relatorios/exportar-pdf?inicio={inicio:yyyy-MM-dd}&fim={fim:yyyy-MM-dd}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }

                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao baixar PDF: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CustoBonificacaoDTO>> ObterCustoIndicacaoAsync(DateTime inicio, DateTime fim, string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string url = $"relatorios/custo-indicacao?inicio={inicio:yyyy-MM-dd}&fim={fim:yyyy-MM-dd}";

                var resultado = await _httpClient.GetAsync(url);

                if (resultado.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    var dados = await resultado.Content.ReadFromJsonAsync<List<CustoBonificacaoDTO>>();
                    return dados ?? new List<CustoBonificacaoDTO>();
                }
                else
                {
                    // Se der erro 401, 500, etc., vai estourar a exceção para aparecer na tela
                    var erro = await resultado.Content.ReadAsStringAsync();
                    throw new Exception($"Status {resultado.StatusCode}. Detalhe: {erro}");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}