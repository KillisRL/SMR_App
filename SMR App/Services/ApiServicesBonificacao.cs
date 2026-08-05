using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
// using SMRDominio.DTOs; // Descomente para importar os DTOs do seu TCC

namespace SMR_App.Services
{
    public class ApiServicesBonificacao
    {
        private readonly HttpClient _httpClient;

        // Mantendo o HTTPS que você corrigiu e apontando para a porta do seu Swagger
        private const string BaseUrl = "https://localhost:7190/";

        public ApiServicesBonificacao()
        {
            // O Handler que driba a rede da faculdade, igual você fez na ConfigEmpresaViewModel
            var handler = new HttpClientHandler { UseProxy = false };
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        // ==========================================================
        // CADASTRAR BONIFICAÇÃO (POST)
        // ==========================================================
        // Troque 'object' pelo seu DTO real, ex: CadastroBonificacaoDTO
        public async Task<RespostaApi> CadastrarBonificacaoService(object dadosCadastro, string token)
        {
            try
            {
                // Injeta o Token JWT no cabeçalho da requisição
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Dispara o POST para o Controller (Ajuste "api/bonificacao" para a rota real do seu Swagger)
                var response = await _httpClient.PostAsJsonAsync("api/bonificacao", dadosCadastro);

                if (response.IsSuccessStatusCode)
                {
                    return new RespostaApi { Sucesso = true, Mensagem = "Bonificação salva na base de dados!" };
                }

                // Se a API retornar BadRequest ou similar, capturamos a mensagem de erro
                var erroApi = await response.Content.ReadAsStringAsync();
                return new RespostaApi { Sucesso = false, Mensagem = $"A API recusou o cadastro: {erroApi}" };
            }
            catch (Exception ex)
            {
                return new RespostaApi { Sucesso = false, Mensagem = ex.Message };
            }
        }

        // ==========================================================
        // ALTERAR BONIFICAÇÃO (PUT)
        // ==========================================================
        public async Task<RespostaApi> AlterarBonificacaoService(object dadosAlteracao, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Dispara o PUT
                var response = await _httpClient.PutAsJsonAsync("api/bonificacao", dadosAlteracao);

                if (response.IsSuccessStatusCode)
                {
                    return new RespostaApi { Sucesso = true, Mensagem = "Dados da bonificação atualizados!" };
                }

                var erroApi = await response.Content.ReadAsStringAsync();
                return new RespostaApi { Sucesso = false, Mensagem = $"Erro da API: {erroApi}" };
            }
            catch (Exception ex)
            {
                return new RespostaApi { Sucesso = false, Mensagem = ex.Message };
            }
        }

        // ==========================================================
        // DELETAR BONIFICAÇÃO (DELETE)
        // ==========================================================
        public async Task<RespostaApi> DeletarBonificacaoService(int idBonificacao, string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Passa o ID na rota para deletar do banco
                var response = await _httpClient.DeleteAsync($"api/bonificacao/{idBonificacao}");

                if (response.IsSuccessStatusCode)
                {
                    return new RespostaApi { Sucesso = true, Mensagem = "Bonificação inativada/excluída!" };
                }

                var erroApi = await response.Content.ReadAsStringAsync();
                return new RespostaApi { Sucesso = false, Mensagem = $"Erro ao excluir: {erroApi}" };
            }
            catch (Exception ex)
            {
                return new RespostaApi { Sucesso = false, Mensagem = ex.Message };
            }
        }
    }

    // ==========================================================
    // CLASSE AUXILIAR DE RESPOSTA
    // ==========================================================
    // Se você já tem uma classe de resposta padrão no seu projeto SMRDominio, 
    // pode apagar esta classe e usar a sua.
    public class RespostaApi
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
    }
}