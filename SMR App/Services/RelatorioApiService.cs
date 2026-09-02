using SMR_App.Models;
using SMRDominio.DTOs; // Ou onde seu CustoBonificacaoDTO estiver compartilhado
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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
                    var dados = await resultado.Content.ReadFromJsonAsync<List<CustoBonificacaoDTO>>();
                    return dados ?? new List<CustoBonificacaoDTO>();
                }

                return new List<CustoBonificacaoDTO>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao buscar relatório: {ex.Message}");
                return new List<CustoBonificacaoDTO>();
            }
        }
    }
}