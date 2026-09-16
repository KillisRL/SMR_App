using SMR_App.Models;
using SMRDominio.ClasseBonificacao;
using SMRDominio.DTOs;
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

        public async Task<List<RankingPromotorDTO>> ObterRankingPromotoresAsync(DateTime inicio, DateTime fim, int status, string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                string url = $"relatorios/ranking-promotores?inicio={inicio:yyyy-MM-dd}&fim={fim:yyyy-MM-dd}&status={status}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var dados = await response.Content.ReadFromJsonAsync<List<RankingPromotorDTO>>(options);
                    return dados ?? new List<RankingPromotorDTO>();
                }
                return new List<RankingPromotorDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar ranking: {ex.Message}");
                return new List<RankingPromotorDTO>();
            }
        }

        public async Task<byte[]> BaixarRankingPromotoresExcelAsync(DateTime dataInicio, DateTime dataFim, int statusId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = $"relatorios/ranking-promotores/excel?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}&status={statusId}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        public async Task<byte[]> BaixarRankingPromotoresPdfAsync(DateTime dataInicio, DateTime dataFim, int statusId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = $"relatorios/ranking-promotores/pdf?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}&status={statusId}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        // 1. Método para carregar o gráfico de linhas
        public async Task<List<MetricaConversaoDTO>> ObterMeticasConversaoAsync(DateTime dataInicio, DateTime dataFim, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Formata a data para yyyy-MM-dd para enviar na URL
                string url = $"relatorios/conversao?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return await response.Content.ReadFromJsonAsync<List<MetricaConversaoDTO>>(options) ?? new List<MetricaConversaoDTO>();
                }
                return new List<MetricaConversaoDTO>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao obter métricas de conversão: {ex.Message}");
                return new List<MetricaConversaoDTO>();
            }
        }

        // 2. Método para baixar o Excel
        public async Task<byte[]> BaixarConversaoExcelAsync(DateTime dataInicio, DateTime dataFim, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = $"relatorios/conversao/excel?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }

        // 3. Método para baixar o PDF
        public async Task<byte[]> BaixarConversaoPdfAsync(DateTime dataInicio, DateTime dataFim, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string url = $"relatorios/conversao/pdf?dataInicio={dataInicio:yyyy-MM-dd}&dataFim={dataFim:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }
            return null;
        }
    }
}